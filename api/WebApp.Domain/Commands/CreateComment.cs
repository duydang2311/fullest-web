using WebApp.Domain.Entities;

namespace WebApp.Domain.Commands;

public sealed record CreateComment(ProjectId ProjectId, TaskId TaskId, UserId AuthorId)
{
    public string? ContentJson { get; init; }
}
