using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.Users.V1;

public static partial class GetUserById
{
    public sealed record Request(UserId UserId, string? Fields) : IProjectableRequest;

    public static async Task<Results<ForbidHttpResult, NotFound, Ok<Projectable>>> HandleAsync(
        [AsParameters] Request request,
        AppDbContext db,
        IProjectionService projectionService,
        CancellationToken ct
    )
    {
        var query = db.Users.Where(a => a.Id == request.UserId);

        if (!string.IsNullOrEmpty(request.Fields))
        {
            query = query.Select(projectionService.Project<User>(request.Fields));
        }

        var user = await query.FirstOrDefaultAsync(ct);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(user.ToProjectable(request.Fields));
    }
}
