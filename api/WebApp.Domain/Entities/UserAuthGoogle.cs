namespace WebApp.Domain.Entities;

public sealed record UserAuthGoogle : UserAuth
{
    public string GoogleId { get; init; } = default!;
}
