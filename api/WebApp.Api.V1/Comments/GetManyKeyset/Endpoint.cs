using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.GetManyKeyset;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Results<NotFound, Ok<KeysetList<Projectable, CommentId>>>>
{
    public override void Configure()
    {
        Get("comments/keyset");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<
        Results<NotFound, Ok<KeysetList<Projectable, CommentId>>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        var query = db.Comments.Where(a => a.DeletedTime == null && a.TaskId == req.TaskId);
        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<Comment>(req.Fields));
        }

        var totalCount = await query.CountAsync(ct).ConfigureAwait(false);
        bool hasNext = false;
        bool hasPrevious = false;
        List<Comment>? list = default;
        if (req.After.HasValue)
        {
            list = await query
                .Where(a => a.Id > req.After.Value)
                .OrderBy(a => a.Id)
                .Take(req.Size + 1)
                .ToListAsync(ct)
                .ConfigureAwait(false);
            hasPrevious = true;
            hasNext = list.Count > req.Size;
        }
        else if (req.Before.HasValue)
        {
            list = await query
                .Where(a => a.Id < req.Before.Value)
                .OrderByDescending(a => a.Id)
                .Take(req.Size + 1)
                .Reverse()
                .ToListAsync(ct)
                .ConfigureAwait(false);
            hasNext = true;
            hasPrevious = list.Count > req.Size;
        }
        else
        {
            list = await query
                .OrderBy(a => a.Id)
                .Take(req.Size + 1)
                .ToListAsync(ct)
                .ConfigureAwait(false);
            hasPrevious = false;
            hasNext = list.Count > req.Size;
        }

        return TypedResults.Ok(
            KeysetList.From(
                list.Take(req.Size).Select(task => task.ToProjectable(req.Fields)),
                hasPrevious,
                hasNext,
                list[0].Id,
                list[^1].Id,
                totalCount
            )
        );
    }
}
