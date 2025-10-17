using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Namespaces.GetOne.BySlug;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Results<NotFound, Ok<Projectable>>>
{
    public override void Configure()
    {
        Get("namespaces/{Slug}");
        Version(1);
    }

    public override async Task<Results<NotFound, Ok<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Namespaces.Where(a => a.User!.Name.Equals(req.Slug));
        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<Namespace>(req.Fields));
        }

        var ns = await query.FirstOrDefaultAsync(ct);
        if (ns is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(ns.ToProjectable(req.Fields));
    }
}
