using System.Security.Claims;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Comments.GetOne;

public sealed record Request(CommentId CommentId, string? Fields)
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
