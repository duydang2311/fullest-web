namespace WebApp.Domain.Entities;

public sealed record Asset
{
    public string Key { get; init; } = null!;
    public string Version { get; init; } = null!;
}
