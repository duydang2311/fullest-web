using WebApp.Domain.Entities;

namespace WebApp.Domain.Constants;

public sealed record StatusDefaults(
    string Name,
    StatusCategory Category,
    string Color,
    string? Description
)
{
    public static readonly StatusDefaults Backlog = new(
        "Backlog",
        StatusCategory.Proposed,
        string.Empty,
        "Tasks not ready or unplanned"
    );
    public static readonly StatusDefaults Todo = new(
        "Todo",
        StatusCategory.Ready,
        string.Empty,
        "Tasks ready to be picked up"
    );
    public static readonly StatusDefaults InProgress = new(
        "In Progress",
        StatusCategory.Active,
        string.Empty,
        "Tasks being worked on"
    );
    public static readonly StatusDefaults Paused = new(
        "Paused",
        StatusCategory.Paused,
        string.Empty,
        "Tasks being on hold"
    );
    public static readonly StatusDefaults Review = new(
        "Review",
        StatusCategory.Review,
        string.Empty,
        "Tasks awaiting for feedback or approval"
    );
    public static readonly StatusDefaults Done = new(
        "Done",
        StatusCategory.Completed,
        string.Empty,
        "Tasks finished successfully"
    );
    public static readonly StatusDefaults Cancelled = new(
        "Cancelled",
        StatusCategory.Canceled,
        string.Empty,
        "Tasks no longer required"
    );

    public static readonly StatusDefaults[] All =
    [
        Backlog,
        Todo,
        InProgress,
        Paused,
        Review,
        Done,
        Cancelled,
    ];
}
