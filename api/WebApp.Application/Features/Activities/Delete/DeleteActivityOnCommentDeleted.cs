using Microsoft.EntityFrameworkCore;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Delete;

public sealed class DeleteActivityOnCommentDeleted(BaseDbContext db) : ICommentDeletedHandler
{
    public async Task HandleAsync(CommentDeleted deleted, CancellationToken ct)
    {
        await db
            .Activities
            .Where(a =>
                a.Kind == ActivityKind.Commented
                && a.Data != null
                && a.Data.RootElement.GetProperty("CommentId").GetInt64() == deleted.CommentId.Value
            )
            .ExecuteDeleteAsync(ct)
            .ConfigureAwait(false);
    }
}
