using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.GetMany;

public sealed record Request(ProjectId? ProjectId, string? Fields, int Page, int Size, string Sort)
    : IOffsetPagination,
        IOrderable
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
