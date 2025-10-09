namespace WebApp.Domain.Entities;

public sealed record Status
{
    public StatusId Id { get; init; }
    public string Name { get; init; } = null!;
    public StatusCategory Category { get; init; }
    public string Color { get; init; } = null!;
    public string Rank { get; init; } = null!;
    public string? Description { get; init; }
}
