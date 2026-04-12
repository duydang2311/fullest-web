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
    : Endpoint<Request, Results<NotFound, Ok<KeysetList<Projectable>>>>
{
    public override void Configure()
    {
        Get("activities");
        Version(1);
    }

    public override async Task<Results<NotFound, Ok<KeysetList<Projectable>>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var baseQuery = db.Activities.AsQueryable();
        if (req.ProjectId.HasValue)
        {
            baseQuery = baseQuery.Where(a => a.ProjectId == req.ProjectId);
        }
        if (req.TaskId.HasValue)
        {
            baseQuery = baseQuery.Where(a => a.TaskId == req.TaskId);
        }
        if (req.ForUserId.HasValue)
        {
            baseQuery = baseQuery.Where(a =>
                db.ProjectMembers.Any(b =>
                    b.UserId == req.ForUserId.Value && b.ProjectId == a.ProjectId
                )
            );
        }

        var query = baseQuery;
        if (req.UntilId.HasValue)
        {
            query = req.Direction.IsAscending
                ? query.Where(a => a.Id <= req.UntilId.Value)
                : query.Where(a => a.Id >= req.UntilId.Value);
            query = query.OrderBy(req.Direction.Reversed(), a => a.Id);
        }
        else
        {
            if (req.AfterId.HasValue)
            {
                query = req.Direction.IsAscending
                    ? query.Where(a => a.Id > req.AfterId.Value)
                    : query.Where(a => a.Id < req.AfterId.Value);
            }
            query = query.OrderBy(req.Direction, a => a.Id);
        }

        query = query.Take(req.Size + 1);

        if (!string.IsNullOrEmpty(req.Select))
        {
            query = query.Select(FieldProjector.Project<Activity>(req.Select));
        }

        var items = await query.ToListAsync(ct).ConfigureAwait(false);
        var hasNext = items.Count > req.Size;
        if (hasNext)
        {
            items.RemoveAt(items.Count - 1);
        }
        var hasPrevious = req.AfterId.HasValue;
        if (req.UntilId.HasValue)
        {
            hasPrevious = await (
                req.Direction.IsAscending
                    ? baseQuery.AnyAsync(a => a.Id > req.UntilId.Value, ct).ConfigureAwait(false)
                    : baseQuery.AnyAsync(a => a.Id < req.UntilId.Value, ct).ConfigureAwait(false)
            );
        }
        var hydrated = await activityHydrator.GetHydratedActivitiesAsync(items, ct);
        return TypedResults.Ok(
            KeysetList.From(
                items: hydrated.Select(Projectable.From<HydratedActivity>(req.Select)),
                hasPrevious: hasPrevious,
                hasNext: hasNext
            )
        );
    }
}
