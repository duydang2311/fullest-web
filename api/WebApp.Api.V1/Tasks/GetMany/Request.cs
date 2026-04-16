using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.GetMany;

public sealed record Request
{
    public ProjectId? ProjectId { get; init; }
    public string? Fields { get; init; }
    public int Size { get; init; } = 20;
    public Direction Direction { get; init; }
    public TaskId? AfterId { get; init; }
    public TaskId? UntilId { get; init; }
    public StatusId? StatusId { get; init; }
    public bool HasStatusFilter { get; init; }
    public bool IncludeTotalCount { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
