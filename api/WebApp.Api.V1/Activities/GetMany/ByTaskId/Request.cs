using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId;

public sealed record Request
{
    public ProjectId? ProjectId { get; init; }
    public TaskId? TaskId { get; init; }
    public UserId? ForUserId { get; init; }
    public string? Select { get; init; }
    public int Size { get; init; } = 20;
    public Direction Direction { get; init; }
    public ActivityId? AfterId { get; init; }
    public ActivityId? UntilId { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
