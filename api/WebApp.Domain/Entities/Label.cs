namespace WebApp.Domain.Entities;

public sealed record Label
{
    public LabelId Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Color { get; init; } = null!;
}
