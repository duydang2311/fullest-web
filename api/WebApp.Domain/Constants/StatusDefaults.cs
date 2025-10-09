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
        StatusCategory.Pending,
        string.Empty,
        "Tasks you think about doing"
    );
    public static readonly StatusDefaults Todo = new(
        "Todo",
        StatusCategory.Pending,
        string.Empty,
        "Tasks you decided to do"
    );
    public static readonly StatusDefaults InProgress = new(
        "In Progress",
        StatusCategory.Active,
        string.Empty,
        "Tasks being worked on"
    );
    public static readonly StatusDefaults Review = new(
        "Review",
        StatusCategory.Active,
        string.Empty,
        "Tasks being reviewed"
    );
    public static readonly StatusDefaults Done = new(
        "Done",
        StatusCategory.Completed,
        string.Empty,
        "Tasks that are finished"
    );
    public static readonly StatusDefaults Cancelled = new(
        "Cancelled",
        StatusCategory.Canceled,
        string.Empty,
        "Tasks that are cancelled"
    );
    public static readonly StatusDefaults Duplicate = new(
        "Duplicate",
        StatusCategory.Canceled,
        string.Empty,
        "Tasks that are duplicated"
    );

    public static readonly StatusDefaults[] All =
    [
        Backlog,
        Todo,
        InProgress,
        Review,
        Done,
        Cancelled,
        Duplicate,
    ];
}
