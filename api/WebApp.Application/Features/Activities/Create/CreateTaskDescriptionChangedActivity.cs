using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskDescriptionChangedActivity(
    BaseDbContext db,
    IOptions<JsonSerializerOptions> jsonSerializerOptions
) : TaskPropertyChangedHandler<TaskDescriptionChanged>
{
    public override async Task HandleAsync(TaskDescriptionChanged changed, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                ActorId = changed.ActorId,
                ProjectId = changed.ProjectId,
                TaskId = changed.TaskId,
                Kind = ActivityKind.TitleChanged,
                Metadata = JsonSerializer.Serialize(
                    new { changed.DescriptionJson, changed.OldDescriptionJson },
                    jsonSerializerOptions.Value
                ),
            },
            ct
        );
    }
}
