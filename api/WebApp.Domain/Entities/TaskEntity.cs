using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record TaskEntity : ISoftDelete
{
    public Instant CreatedTime { get; init; }
    public Instant UpdatedTime { get; init; }
    public TaskId Id { get; init; }
    public ProjectId ProjectId { get; init; }
    public Project Project { get; init; } = null!;
    public UserId AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public ICollection<User> Assignees { get; init; } = null!;
    public long PublicId { get; init; }
    public string Title { get; init; } = null!;
    public StatusId? StatusId { get; init; }
    public Status? Status { get; init; }
    public PriorityId? PriorityId { get; init; }
    public Priority? Priority { get; init; }
    public CommentId? InitialCommentId { get; init; }
    public Comment? InitialComment { get; init; } = null!;
    public ICollection<Comment>? Comments { get; init; }
    public Instant? DueTime { get; init; }
    public string? DueTz { get; init; }
    public Instant? DeletedTime { get; init; }
    public ICollection<Label> Labels { get; init; } = null!;
}
