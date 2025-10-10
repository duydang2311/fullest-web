namespace WebApp.Domain.Constants;

public sealed record PriorityDefaults(string Name, string Color, string? Description)
{
    public static readonly PriorityDefaults Low = new(
        "Low",
        string.Empty,
        "Tasks with minimal impact, can be done later."
    );
    public static readonly PriorityDefaults Medium = new(
        "Medium",
        string.Empty,
        "Important tasks to handle in normal workflow."
    );
    public static readonly PriorityDefaults High = new(
        "High",
        string.Empty,
        "Critical tasks that should be done soon."
    );
    public static readonly PriorityDefaults Urgent = new(
        "Urgent",
        string.Empty,
        "Tasks needing immediate action."
    );

    public static readonly PriorityDefaults[] All = [Low, Medium, High, Urgent];
}
