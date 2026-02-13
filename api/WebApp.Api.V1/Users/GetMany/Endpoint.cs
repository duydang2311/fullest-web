using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Users.GetMany;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Results<NotFound, Ok<CursorList<Projectable, UserId?>>>>
{
    public override void Configure()
    {
        Get("users");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<
        Results<NotFound, Ok<CursorList<Projectable, UserId?>>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        var query = db.Users.Where(a => a.DeletedTime == null);

        if (!string.IsNullOrEmpty(req.Search))
        {
            query = db
                .Users.Where(a =>
                    a.DisplayName == null
                        ? EF.Functions.TrigramsSimilarity(a.Name, req.Search) >= 0.3
                        : (
                            EF.Functions.TrigramsSimilarity(a.Name, req.Search)
                            + EF.Functions.TrigramsSimilarity(a.DisplayName, req.Search) / 2
                        ) >= 0.3
                )
                .OrderByDescending(a =>
                    a.DisplayName == null
                        ? EF.Functions.TrigramsSimilarity(a.Name, req.Search)
                        : (
                            EF.Functions.TrigramsSimilarity(a.Name, req.Search)
                            + EF.Functions.TrigramsSimilarity(a.DisplayName, req.Search) / 2
                        )
                )
                .ThenBy(a => a.Id);
        }
        else
        {
            query = query.SortOrDefault(Orderable.From(req), a => a.OrderBy(a => a.Id));
        }
        if (req.After.HasValue)
        {
            query = query.Where(a => a.Id > req.After.Value);
        }
        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<User>(req.Fields));
        }

        var users = await query.Take(req.Size + 1).ToListAsync(ct).ConfigureAwait(false);

        if (users is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(
            CursorList.From(
                users.Take(req.Size).Select(a => a.ToProjectable(req.Fields)),
                (UserId?)users[^1].Id,
                users.Count > req.Size
            )
        );
    }
}
