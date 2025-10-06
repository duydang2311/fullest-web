namespace WebApp.Domain.Entities;

public sealed record ProjectStatus
{
    public ProjectStatusId Id { get; init; }
    public ProjectId ProjectId { get; init; }
    public Project Project { get; init; } = null!;
    public StatusId StatusId { get; init; }
    public Status Status { get; init; } = null!;
    public bool IsDefault { get; init; }
}
