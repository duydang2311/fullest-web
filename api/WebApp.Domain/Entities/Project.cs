using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record Project : ISoftDelete
{
    public Instant CreatedTime { get; init; }
    public NamespaceId NamespaceId { get; init; }
    public Namespace Namespace { get; init; } = null!;
    public ProjectId Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Summary { get; init; }
    public string? About { get; init; }
    public string Identifier { get; init; } = null!;
    public Instant? DeletedTime { get; init; }
    public ICollection<ProjectMember> ProjectMembers { get; init; } = null!;
}
