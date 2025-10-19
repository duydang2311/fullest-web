using System.Security.Claims;
using System.Text.Json;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Comments.Create;

public sealed record Request(TaskId TaskId, JsonDocument? ContentJson, string? ContentText)
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
