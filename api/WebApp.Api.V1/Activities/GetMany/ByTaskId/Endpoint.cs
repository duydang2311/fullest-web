using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId;

public sealed class Endpoint(AppDbContext db, ActivityHydrator activityHydrator)
    : Endpoint<Request, Results<NotFound, Ok<CursorList<Projectable, ActivityId?>>>>
{
    public override void Configure()
    {
        Get("activities");
        Version(1);
    }

    public override async Task<
        Results<NotFound, Ok<CursorList<Projectable, ActivityId?>>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        var query = db.Activities.AsQueryable();
        if (req.TaskId.HasValue)
        {
            query = query.Where(a => a.TaskId == req.TaskId.Value);
        }
        if (req.After.HasValue)
        {
            query = query.Where(a => a.Id > req.After.Value).OrderBy(a => a.Id);
        }

        query = query.Take(req.Size + 1);

        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<Activity>(req.Fields));
        }

        var items = await query.ToListAsync(ct).ConfigureAwait(false);
        var hasMore = items.Count > req.Size;
        var hydrated = await activityHydrator.GetHydratedActivitiesAsync(
            hasMore ? [.. items.Take(req.Size)] : items,
            ct
        );
        return TypedResults.Ok(
            CursorList.From(
                hydrated.Select(Projectable.From<Activity>(null)),
                (ActivityId?)hydrated[^1].Id,
                hasMore
            )
        );
    }
}
