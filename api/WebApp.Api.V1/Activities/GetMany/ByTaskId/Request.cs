using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId;

public sealed record Request : IOrderable
{
    public ProjectId? ProjectId { get; init; }
    public TaskId? TaskId { get; init; }
    public UserId? ForUserId { get; init; }
    public string? Select { get; init; }
    public ActivityId? After { get; init; }
    public int Size { get; init; } = 20;
    public string Sort { get; init; } = string.Empty;

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
