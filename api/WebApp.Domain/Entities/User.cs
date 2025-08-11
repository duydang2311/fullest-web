using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record User : ISoftDelete
{
    public UserId Id { get; init; }
    public string Name { get; init; } = null!;
    public ICollection<UserAuth> Auths { get; init; } = null!;
    public Instant? DeletedTime { get; init; }
}
