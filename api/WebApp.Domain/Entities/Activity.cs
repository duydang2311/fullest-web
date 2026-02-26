using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record Activity
{
    public Instant CreatedTime { get; init; }
    public ActivityId Id { get; init; }
    public UserId? ActorId { get; init; }
    public User? Actor { get; init; }
    public ProjectId? ProjectId { get; init; }
    public Project? Project { get; init; }
    public TaskId? TaskId { get; init; }
    public TaskEntity? Task { get; init; }
    public ActivityKind Kind { get; init; }
    public string? Metadata { get; init; }
}
