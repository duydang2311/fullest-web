namespace WebApp.Domain.Entities;

public enum StatusCategory : byte
{
    Proposed = 1,
    Ready,
    Active,
    Paused,
    Review,
    Completed,
    Canceled,
    Archived,
}
