using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record UserProfile
{
    public Instant CreatedTime { get; init; }
    public User User { get; init; } = null!;
    public UserId UserId { get; init; }
    public string? DisplayName { get; init; }
    public string? ImageKey { get; init; }
    public string? ImageVersion { get; init; }
}
