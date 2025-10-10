using System.Security.Claims;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.GetOne.ByPublicId;

public sealed record Request(ProjectId ProjectId, long PublicId, string? Fields)
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
