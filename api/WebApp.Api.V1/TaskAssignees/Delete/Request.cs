using System.Security.Claims;
using FastEndpoints;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.TaskAssignees.Delete;

public sealed record Request
{
    public TaskId TaskId { get; init; }
    public UserId UserId { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
