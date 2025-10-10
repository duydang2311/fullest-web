using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.GetMany;

public sealed record Request : IOffsetPagination, IOrderable
{
    public ProjectId? ProjectId { get; init; }
    public string? Fields { get; init; }
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 20;
    public string Sort { get; init; } = string.Empty;

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
