using System.Security.Claims;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.GetOne.ById;

public sealed record Request(ProjectId ProjectId, TaskId TaskId, string? Fields)
{
    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
