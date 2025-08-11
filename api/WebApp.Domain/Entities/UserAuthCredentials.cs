namespace WebApp.Domain.Entities;

public sealed record UserAuthCredentials : UserAuth
{
    public byte[] Hash { get; init; } = default!;
}
