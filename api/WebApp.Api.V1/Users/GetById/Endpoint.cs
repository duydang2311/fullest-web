using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Users.GetById;

public sealed class Endpoint(AppDbContext db, IProjectionService projectionService)
    : Endpoint<Request, Results<NotFound, Ok<Projectable>>>
{
    public override void Configure()
    {
        Get("users/{UserId}");
        Version(1);
    }

    public override async Task<Results<NotFound, Ok<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Users.Where(a => a.Id == req.UserId);

        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(projectionService.Project<User>(req.Fields));
        }

        var user = await query.FirstOrDefaultAsync(ct);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(user.ToProjectable(req.Fields));
    }
}
