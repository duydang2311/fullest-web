using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Users.GetMany;

public sealed record Request : ICursorPagination<UserId?>, IOrderable
{
    public string? Search { get; init; }
    public string? Fields { get; init; }
    public UserId? Cursor { get; init; }
    public int Size { get; init; } = 20;
    public string? Sort { get; init; }
}
