using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.GetOne;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Results<NotFound, Ok<Projectable>>>
{
    public override void Configure()
    {
        Get("comments/{CommentId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Ok<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Comments.Where(a => a.DeletedTime == null && a.Id == req.CommentId);
        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<Comment>(req.Fields));
        }
        var comment = await query.FirstOrDefaultAsync(ct).ConfigureAwait(false);
        if (comment is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(comment.ToProjectable(req.Fields));
    }
}
