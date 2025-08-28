using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record Project : ISoftDelete
{
    public Instant CreatedTime { get; init; }
    public UserId CreatorId { get; init; }
    public User Creator { get; init; } = null!;
    public ProjectId Id { get; init; }
    public string Name { get; init; } = null!;
    public string Identifier { get; init; } = null!;
    public Instant? DeletedTime { get; init; }
    public ICollection<ProjectMember> ProjectMembers { get; init; } = null!;
}
