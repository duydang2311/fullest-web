using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.Delete;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Results<NotFound, Ok>>
{
    public override void Configure()
    {
        Delete("comments/{CommentId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Ok>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var count = await db
            .Comments.Where(a => a.Id == req.CommentId)
            .ExecuteDeleteAsync(ct)
            .ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok();
    }
}
