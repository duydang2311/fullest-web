namespace WebApp.Domain.Entities;

public enum ActivityKind
{
    Created = 1,

    Commented = 200,
    Assigned = 201,
    StatusChanged = 202,
    PriorityChanged = 203,
    TitleChanged = 204,
    DueTimeChanged = 205,
    Unassigned = 206,
}
