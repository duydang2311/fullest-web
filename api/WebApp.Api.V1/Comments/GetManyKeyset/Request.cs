using System.Security.Claims;
using FastEndpoints;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Comments.GetManyKeyset;

public sealed record Request : IKeysetPagination<CommentId?>
{
    public TaskId TaskId { get; init; }
    public string? Fields { get; init; }
    public CommentId? After { get; init; }
    public CommentId? Before { get; init; }
    public int Size { get; init; } = 20;

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}
