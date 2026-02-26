using System.Text.Json;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskTitleChangedActivity(BaseDbContext db)
    : TaskPropertyChangedHandler<TaskTitleChanged>
{
    public override async Task HandleAsync(TaskTitleChanged changed, CancellationToken ct)
    {
        await db.Activities.AddAsync(
            new Activity
            {
                ActorId = changed.ActorId,
                ProjectId = changed.ProjectId,
                TaskId = changed.TaskId,
                Kind = ActivityKind.TitleChanged,
                Metadata = JsonSerializer.Serialize(new { changed.Title, changed.OldTitle }),
            },
            ct
        );
    }
}
