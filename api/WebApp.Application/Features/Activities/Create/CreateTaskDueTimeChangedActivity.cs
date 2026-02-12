using System.Text.Json;
using NodaTime;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskDueTimeChangedActivity(BaseDbContext db)
    : TaskPropertyChangedHandler<TaskDueTimeChanged>
{
    public override async Task HandleAsync(TaskDueTimeChanged changed, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                TaskId = changed.TaskId,
                ActorId = changed.ActorId,
                Kind = ActivityKind.DueTimeChanged,
                Data = JsonSerializer.SerializeToDocument(
                    new
                    {
                        DueTime = changed.DueTime.HasValue ? (Instant?)changed.DueTime.Value : null,
                        changed.DueTz,
                        OldDueTime = changed.OldDueTime.HasValue
                            ? (Instant?)changed.OldDueTime.Value
                            : null,
                        changed.OldDueTz,
                    }
                ),
            },
            ct
        );
    }
}
