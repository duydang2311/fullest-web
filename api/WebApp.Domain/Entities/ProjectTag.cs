namespace WebApp.Domain.Entities;

public sealed record ProjectTag
{
    public ProjectId ProjectId { get; init; }
    public Project Project { get; init; } = null!;
    public TagId TagId { get; init; }
    public Tag Tag { get; init; } = null!;
}
