using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.GetMany;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Ok<KeysetList<Projectable>>>
{
    public override void Configure()
    {
        Get("tasks");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Ok<KeysetList<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var baseQuery = db.Tasks.Where(a => a.DeletedTime == null);
        if (req.ProjectId.HasValue)
        {
            baseQuery = baseQuery.Where(a =>
                a.ProjectId == req.ProjectId.Value && a.Project.DeletedTime == null
            );
        }
        if (req.HasStatusFilter)
        {
            baseQuery = baseQuery.Where(a => a.StatusId == req.StatusId);
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

        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<TaskEntity>(req.Fields));
        }

        var list = await query.Take(req.Size + 1).ToListAsync(ct).ConfigureAwait(false);
        var hasPrevious = req.AfterId is not null;
        if (req.UntilId.HasValue)
        {
            var hasPreviousQuery = req.Direction.IsDescending
                ? baseQuery.Where(a => a.Id > req.UntilId.Value)
                : baseQuery.Where(a => a.Id < req.UntilId.Value);
            hasPrevious = await hasPreviousQuery.AnyAsync(ct).ConfigureAwait(false);
        }
        var hasNext = list.Count > req.Size;
        if (hasNext)
        {
            list.RemoveAt(list.Count - 1);
        }
        var totalCount = req.IncludeTotalCount
            ? await baseQuery.CountAsync(ct).ConfigureAwait(false)
            : 0;
        return TypedResults.Ok(
            KeysetList.From(
                items: list.Select(task => task.ToProjectable(req.Fields)),
                hasPrevious: hasPrevious,
                hasNext: hasNext,
                totalCount: totalCount
            )
        );
    }
}
