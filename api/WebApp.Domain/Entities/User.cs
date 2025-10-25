using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record User : ISoftDelete
{
    public Instant CreatedTime { get; init; }
    public UserId Id { get; init; }
    public string Name { get; init; } = null!;
    public ICollection<UserAuth> Auths { get; init; } = null!;
    public Instant? DeletedTime { get; init; }
    public ICollection<Project> Projects { get; init; } = null!;
    public ICollection<ProjectMember> ProjectMembers { get; init; } = null!;
    public UserProfile? Profile { get; init; }
}
