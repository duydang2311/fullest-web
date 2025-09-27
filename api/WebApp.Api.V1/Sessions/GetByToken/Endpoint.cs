using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Sessions.GetByToken;

public sealed class Endpoint(AppDbContext db, IProjectionService projectionService)
    : Endpoint<Request, Results<NotFound, Ok<Projectable>>>
{
    public override void Configure()
    {
        Get("sessions/{Token}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task<Results<NotFound, Ok<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var decoded = WebEncoders.Base64UrlDecode(req.Token);
        var query = db.UserSessions.Where(a => a.Token.SequenceEqual(decoded));

        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(projectionService.Project<UserSession>(req.Fields));
        }

        var user = await query.FirstOrDefaultAsync(ct);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(user.ToProjectable(req.Fields));
    }
}
