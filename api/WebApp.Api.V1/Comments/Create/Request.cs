using System.Security.Claims;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Comments.Create;

public sealed record Request(TaskId TaskId, string? ContentJson)
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
