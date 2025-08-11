using WebApp.Domain.Constants;

namespace WebApp.Domain.Entities;

public abstract record UserAuth
{
    public UserAuthId Id { get; init; }
    public UserId UserId { get; init; }
    public User User { get; init; } = default!;
    public AuthProvider Provider { get; init; }
}
