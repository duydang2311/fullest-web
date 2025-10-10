namespace WebApp.Domain.Entities;

public sealed record Priority
{
    public PriorityId Id { get; init; }
    public Project Project { get; init; } = null!;
    public ProjectId ProjectId { get; init; }
    public string Name { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Rank { get; init; } = null!;
    public string? Description { get; init; }
}
