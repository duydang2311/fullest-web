namespace WebApp.Domain.Entities;

public sealed record UserAuthCredentials : UserAuth
{
    public string Hash { get; init; } = null!;
}
