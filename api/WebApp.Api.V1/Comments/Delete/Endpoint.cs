using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Events;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.Delete;

public sealed class Endpoint(AppDbContext db, IEnumerable<ICommentDeletedHandler> commentDeletedHandlers)
    : Endpoint<Request, Results<NotFound, Ok>>
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
        await using var transaction = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        var count = await db
            .Comments.Where(a => a.Id == req.CommentId)
            .ExecuteDeleteAsync(ct)
            .ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.NotFound();
        }

        var commentDeleted = new CommentDeleted(req.CommentId);
        foreach (var handler in commentDeletedHandlers)
        {
            await handler.HandleAsync(commentDeleted, ct).ConfigureAwait(false);
        }

        await transaction.CommitAsync(ct).ConfigureAwait(false);
        return TypedResults.Ok();
    }
}
