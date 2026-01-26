using System.Security.Claims;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.Delete;

public sealed record Request(TaskId TaskId)
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
