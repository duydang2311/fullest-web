namespace WebApp.Domain.Entities;

public sealed record TaskLabel
{
    public TaskId TaskId { get; init; }
    public TaskEntity Task { get; init; } = null!;
    public LabelId LabelId { get; init; }
    public Label Label { get; init; } = null!;
}
