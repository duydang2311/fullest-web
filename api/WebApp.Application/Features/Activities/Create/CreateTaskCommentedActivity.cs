using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskCommentedActivity(
    BaseDbContext db,
    IOptions<JsonSerializerOptions> jsonSerializerOptions
) : ICommentCreatedHandler
{
    public async Task HandleAsync(CommentCreated created, CancellationToken ct)
    {
        await db.AddAsync(
                new Activity
                {
                    Kind = ActivityKind.Commented,
                    ProjectId = created.ProjectId,
                    TaskId = created.Comment.TaskId,
                    ActorId = created.Comment.AuthorId,
                    Metadata = JsonSerializer.Serialize(
                        new { CommentId = created.Comment.Id.Value },
                        jsonSerializerOptions.Value
                    ),
                },
                ct
            )
            .ConfigureAwait(false);
    }
}
