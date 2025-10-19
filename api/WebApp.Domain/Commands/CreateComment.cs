using System.Text.Json;
using WebApp.Domain.Entities;

namespace WebApp.Domain.Commands;

public sealed record CreateComment(TaskId TaskId, UserId AuthorId)
{
    public JsonDocument? ContentJson { get; init; }
    public string? ContentText { get; init; }
}
