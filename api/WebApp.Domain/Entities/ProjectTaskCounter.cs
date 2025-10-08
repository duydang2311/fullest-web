namespace WebApp.Domain.Entities;

public sealed record ProjectTaskCounter
{
    public ProjectId ProjectId { get; init; }
    public long Count { get; init; }
}
