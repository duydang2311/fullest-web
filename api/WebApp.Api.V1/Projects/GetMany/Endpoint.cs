using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Projects.GetMany;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Ok<KeysetList<Projectable>>>
{
    public override void Configure()
    {
        Get("projects");
        Version(1);
    }

    public override async Task<Ok<KeysetList<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        Guard.Against.Null(req.MemberId);
        var baseQuery = db.Projects.Where(a =>
            a.DeletedTime == null && a.ProjectMembers.Any(b => b.UserId == req.MemberId.Value)
        );
        var query = baseQuery;

        if (req.AfterId.HasValue)
        {
            query = req.Direction.IsDescending
                ? query.Where(a => a.Id < req.AfterId.Value)
                : query.Where(a => a.Id > req.AfterId.Value);
        }
        else if (req.UntilId.HasValue)
        {
            query = req.Direction.IsDescending
                ? query.Where(a => a.Id <= req.UntilId.Value)
                : query.Where(a => a.Id >= req.UntilId.Value);
        }
        query = req.Direction.IsDescending
            ? query.OrderByDescending(a => a.Id)
            : query.OrderBy(a => a.Id);

        if (!string.IsNullOrEmpty(req.Select))
        {
            query = query.Select(FieldProjector.Project<Project>(req.Select));
        }

        var list = await query.Take(req.Size + 1).ToListAsync(ct).ConfigureAwait(false);
        var hasPrevious = req.AfterId is not null;
        if (req.UntilId.HasValue)
        {
            var previousQuery = req.Direction.IsDescending
                ? baseQuery.Where(a => a.Id > req.UntilId.Value)
                : baseQuery.Where(a => a.Id < req.UntilId.Value);
            hasPrevious = await previousQuery.AnyAsync(ct).ConfigureAwait(false);
        }
        var hasNext = list.Count > req.Size;
        if (hasNext)
        {
            list.RemoveAt(list.Count - 1);
        }
        return TypedResults.Ok(
            KeysetList.From(
                items: list.Select(task => task.ToProjectable(req.Select)),
                hasPrevious: hasPrevious,
                hasNext: hasNext
            )
        );
    }
}
