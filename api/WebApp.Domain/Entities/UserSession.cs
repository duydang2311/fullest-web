using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record UserSession
{
    public Instant CreatedTime { get; init; }
    public UserSessionId Id { get; init; }
    public UserId UserId { get; init; }
    public User User { get; init; } = null!;
    public byte[] Token { get; init; } = null!;
}
