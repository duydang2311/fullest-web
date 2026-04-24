using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskAssignedActivity(
    BaseDbContext db,
    IOptions<JsonSerializerOptions> jsonSerializerOptions
) : TaskPropertyChangedHandler<TaskAssigned>, IEventHandler<TaskAssigned>
{
    public override async Task HandleAsync(TaskAssigned assigned, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                ActorId = assigned.ActorId,
                Kind = ActivityKind.Assigned,
                ProjectId = assigned.ProjectId,
                TaskId = assigned.TaskId,
                Metadata = JsonSerializer.Serialize(
                    new { AssigneeId = assigned.AssigneeId.Value },
                    jsonSerializerOptions.Value
                ),
            },
            ct
        );
    }
}
