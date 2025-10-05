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
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public Instant? DeletedTime { get; init; }
}
