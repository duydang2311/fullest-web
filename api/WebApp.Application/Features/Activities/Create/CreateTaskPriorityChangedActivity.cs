using System.Text.Json;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskPriorityChangedActivity(BaseDbContext db)
    : TaskPropertyChangedHandler<TaskPriorityChanged>
{
    public override async Task HandleAsync(TaskPriorityChanged changed, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                TaskId = changed.TaskId,
                ActorId = changed.ActorId,
                Kind = ActivityKind.PriorityChanged,
                Data = JsonSerializer.SerializeToDocument(
                    new
                    {
                        PriorityId = changed.PriorityId?.Value,
                        OldPriorityId = changed.OldPriorityId?.Value,
                    }
                ),
            },
            ct
        );
    }
}
