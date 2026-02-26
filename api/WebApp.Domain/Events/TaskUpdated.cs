using NodaTime;
using WebApp.Domain.Entities;

namespace WebApp.Domain.Events;

public sealed record TaskUpdated(
    ProjectId ProjectId,
    TaskId TaskId,
    IReadOnlyCollection<TaskPropertyChanged> Changes
);

public abstract record TaskPropertyChanged(ProjectId ProjectId, TaskId TaskId, UserId ActorId);

public sealed record TaskAssigned(
    ProjectId ProjectId,
    TaskId TaskId,
    UserId ActorId,
    UserId AssigneeId
) : TaskPropertyChanged(ProjectId, TaskId, ActorId);

public sealed record TaskStatusChanged(
    ProjectId ProjectId,
    TaskId TaskId,
    UserId ActorId,
    StatusId? StatusId,
    StatusId? OldStatusId
) : TaskPropertyChanged(ProjectId, TaskId, ActorId);

public sealed record TaskPriorityChanged(
    ProjectId ProjectId,
    TaskId TaskId,
    UserId ActorId,
    PriorityId? PriorityId,
    PriorityId? OldPriorityId
) : TaskPropertyChanged(ProjectId, TaskId, ActorId);

public sealed record TaskDueTimeChanged(
    ProjectId ProjectId,
    TaskId TaskId,
    UserId ActorId,
    Instant? DueTime,
    string? DueTz,
    Instant? OldDueTime,
    string? OldDueTz
) : TaskPropertyChanged(ProjectId, TaskId, ActorId);

public sealed record TaskTitleChanged(
    ProjectId ProjectId,
    TaskId TaskId,
    UserId ActorId,
    string Title,
    string OldTitle
) : TaskPropertyChanged(ProjectId, TaskId, ActorId);

public sealed record TaskUnassigned(
    ProjectId ProjectId,
    TaskId TaskId,
    UserId ActorId,
    UserId AssigneeId
) : TaskPropertyChanged(ProjectId, TaskId, ActorId);
