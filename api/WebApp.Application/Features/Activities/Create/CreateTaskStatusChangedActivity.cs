using System.Text.Json;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskStatusChangedActivity(BaseDbContext db)
    : TaskPropertyChangedHandler<TaskStatusChanged>
{
    public override async Task HandleAsync(TaskStatusChanged changed, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                TaskId = changed.TaskId,
                ActorId = changed.ActorId,
                Kind = ActivityKind.StatusChanged,
                Data = JsonSerializer.SerializeToDocument(
                    new
                    {
                        StatusId = changed.StatusId?.Value,
                        OldStatusId = changed.OldStatusId?.Value,
                    }
                ),
            },
            ct
        );
    }
}
