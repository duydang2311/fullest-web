using NodaTime;
using WebApp.Domain.Entities;

namespace WebApp.Domain.Events;

public sealed record TaskUpdated(TaskId TaskId, IReadOnlyCollection<TaskPropertyChanged> Changes);

public abstract record TaskPropertyChanged(TaskId TaskId, UserId ActorId);

public sealed record TaskAssigned(TaskId TaskId, UserId ActorId, UserId AssigneeId)
    : TaskPropertyChanged(TaskId, ActorId);

public sealed record TaskStatusChanged(
    TaskId TaskId,
    UserId ActorId,
    StatusId? StatusId,
    StatusId? OldStatusId
) : TaskPropertyChanged(TaskId, ActorId);

public sealed record TaskPriorityChanged(
    TaskId TaskId,
    UserId ActorId,
    PriorityId? PriorityId,
    PriorityId? OldPriorityId
) : TaskPropertyChanged(TaskId, ActorId);

public sealed record TaskDueTimeChanged(
    TaskId TaskId,
    UserId ActorId,
    Instant? DueTime,
    string? DueTz,
    Instant? OldDueTime,
    string? OldDueTz
) : TaskPropertyChanged(TaskId, ActorId);

public sealed record TaskTitleChanged(TaskId TaskId, UserId ActorId, string Title, string OldTitle)
    : TaskPropertyChanged(TaskId, ActorId);

public sealed record TaskUnassigned(TaskId TaskId, UserId ActorId, UserId AssigneeId)
    : TaskPropertyChanged(TaskId, ActorId);
