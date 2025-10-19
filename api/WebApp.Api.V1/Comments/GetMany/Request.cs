using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Comments.GetMany;

public sealed record Request : IOffsetPagination, IOrderable
{
    public TaskId TaskId { get; init; }
    public string? Sort { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
    public string? Fields { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
