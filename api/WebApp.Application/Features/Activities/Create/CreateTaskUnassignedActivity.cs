using System.Text.Json;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskUnassignedActivity(BaseDbContext db)
    : TaskPropertyChangedHandler<TaskUnassigned>,
        IEventHandler<TaskUnassigned>
{
    public override async Task HandleAsync(TaskUnassigned assigned, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                ActorId = assigned.ActorId,
                Kind = ActivityKind.Unassigned,
                ProjectId = assigned.ProjectId,
                TaskId = assigned.TaskId,
                Metadata = JsonSerializer.Serialize(new { AssigneeId = assigned.AssigneeId.Value }),
            },
            ct
        );
    }
}
