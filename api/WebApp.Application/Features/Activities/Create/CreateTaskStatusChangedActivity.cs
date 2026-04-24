using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskStatusChangedActivity(
    BaseDbContext db,
    IOptions<JsonSerializerOptions> jsonSerializerOptions
) : TaskPropertyChangedHandler<TaskStatusChanged>
{
    public override async Task HandleAsync(TaskStatusChanged changed, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                ActorId = changed.ActorId,
                Kind = ActivityKind.StatusChanged,
                ProjectId = changed.ProjectId,
                TaskId = changed.TaskId,
                Metadata = JsonSerializer.Serialize(
                    new
                    {
                        StatusId = changed.StatusId?.Value,
                        OldStatusId = changed.OldStatusId?.Value,
                    },
                    jsonSerializerOptions.Value
                ),
            },
            ct
        );
    }
}
