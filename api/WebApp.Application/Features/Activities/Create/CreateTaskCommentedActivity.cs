using System.Text.Json;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskCommentedActivity(BaseDbContext db) : ICommentCreatedHandler
{
    public async Task HandleAsync(CommentCreated created, CancellationToken ct)
    {
        await db.AddAsync(
                new Activity
                {
                    Kind = ActivityKind.Commented,
                    TaskId = created.Comment.TaskId,
                    ActorId = created.Comment.AuthorId,
                    Data = JsonSerializer.SerializeToDocument(
                        new { CommentId = created.Comment.Id.Value }
                    ),
                },
                ct
            )
            .ConfigureAwait(false);
    }
}
