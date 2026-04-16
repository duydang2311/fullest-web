using WebApp.Domain.Entities;

namespace WebApp.Domain.Constants;

public sealed record PriorityDefaults(
    string Name,
    PriorityCategory Category,
    string Color,
    string? Description
)
{
    public static readonly PriorityDefaults Low = new(
        "Low",
        PriorityCategory.Low,
        string.Empty,
        "Tasks with minimal impact, can be done later."
    );
    public static readonly PriorityDefaults Medium = new(
        "Medium",
        PriorityCategory.Medium,
        string.Empty,
        "Important tasks to handle in normal workflow."
    );
    public static readonly PriorityDefaults High = new(
        "High",
        PriorityCategory.High,
        string.Empty,
        "Critical tasks that should be done soon."
    );
    public static readonly PriorityDefaults Urgent = new(
        "Urgent",
        PriorityCategory.Urgent,
        string.Empty,
        "Tasks needing immediate action."
    );

    public static readonly PriorityDefaults[] All = [Low, Medium, High, Urgent];
}
