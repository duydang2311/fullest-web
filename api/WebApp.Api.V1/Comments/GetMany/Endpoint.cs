using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.GetMany;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Results<NotFound, Ok<OffsetList<Projectable>>>>
{
    public override void Configure()
    {
        Get("comments");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Ok<OffsetList<Projectable>>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Comments.Where(a => a.DeletedTime == null && a.TaskId == req.TaskId);
        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<Comment>(req.Fields));
        }

        var totalCount = await query.CountAsync(ct).ConfigureAwait(false);

        var list = await query.Sort(req).Paginate(req).ToListAsync(ct).ConfigureAwait(false);
        return TypedResults.Ok(
            OffsetList.From(list.Select(task => task.ToProjectable(req.Fields)), req, totalCount)
        );
    }
}
