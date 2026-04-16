using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.GetManyGroupedByStatus;

public sealed record Request
{
    public ProjectId? ProjectId { get; init; }
    public string? Select { get; init; }
    public int Size { get; init; } = 20;
    public Direction Direction { get; init; }
    public bool IncludeTotalCount { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
