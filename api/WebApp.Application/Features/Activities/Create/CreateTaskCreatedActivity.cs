using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskCreatedActivity(BaseDbContext db) : ITaskCreatedHandler
{
    public async Task HandleAsync(TaskCreated created, CancellationToken ct)
    {
        await db.AddAsync(
                new Activity
                {
                    Kind = ActivityKind.Created,
                    TaskId = created.Task.Id,
                    ActorId = created.Task.AuthorId,
                },
                ct
            )
            .ConfigureAwait(false);
    }
}
