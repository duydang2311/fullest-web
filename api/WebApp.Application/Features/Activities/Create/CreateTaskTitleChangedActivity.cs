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
                TaskId = changed.TaskId,
                ActorId = changed.ActorId,
                Kind = ActivityKind.TitleChanged,
                Data = JsonSerializer.SerializeToDocument(new { changed.Title, changed.OldTitle }),
            },
            ct
        );
    }
}
