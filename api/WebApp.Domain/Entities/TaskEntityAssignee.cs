namespace WebApp.Domain.Entities;

public sealed record TaskEntityAssignee
{
    public TaskId TaskId { get; init; }
    public TaskEntity Task { get; init; } = null!;
    public UserId UserId { get; init; }
    public User User { get; init; } = null!;
}
