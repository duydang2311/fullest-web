using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record Comment : ISoftDelete
{
    public Instant CreatedTime { get; init; }
    public TaskId TaskId { get; init; }
    public TaskEntity Task { get; init; } = null!;
    public CommentId Id { get; init; }
    public UserId CreatorId { get; init; }
    public User Creator { get; init; } = null!;
    public string? ContentJson { get; init; }
    public string? ContentPreview { get; init; }
    public Instant? DeletedTime { get; init; }
}
