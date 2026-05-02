using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApp.Application.Common;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Projects.Patch;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Results<NotFound, Conflict, Ok<Response>>>
{
    public override void Configure()
    {
        Patch("projects/{ProjectId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Conflict, Ok<Response>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        Guard.Against.Null(req.Patch);

        Action<UpdateSettersBuilder<Project>>? updateBuilder = null;

        if (req.Patch.TryGetValue(a => a.Name, out var name))
        {
            updateBuilder += a => a.SetProperty(b => b.Name, Guard.Against.Null(name));
        }
        if (req.Patch.TryGetValue(a => a.Summary, out var summary))
        {
            updateBuilder += a => a.SetProperty(b => b.Summary, summary?.Trim());
        }
        if (req.Patch.TryGetValue(a => a.DescriptionJson, out var descriptionJson))
        {
            var (json, preview) = TextDocumentHelper.ParseDocumentPreview(descriptionJson);
            updateBuilder += a =>
                a.SetProperty(b => b.DescriptionJson, json)
                    .SetProperty(b => b.DescriptionPreview, preview);
        }

        Guard.Against.Null(updateBuilder);
        var useOptimisticLock = req.Patch.Has(a => a.DescriptionJson);
        var newVersion = req.Version + 1;
        updateBuilder += a => a.SetProperty(b => b.Version, newVersion);

        if (useOptimisticLock)
        {
            var version = await db
                .Projects.Where(a => a.Id == req.ProjectId && a.DeletedTime == null)
                .Select(a => (uint?)a.Version)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
            if (!version.HasValue)
            {
                return TypedResults.NotFound();
            }
            if (version.Value != req.Version)
            {
                return TypedResults.Conflict();
            }
        }

        var query = db.Projects.Where(a => a.Id == req.ProjectId && a.DeletedTime == null);
        if (useOptimisticLock)
        {
            query = query.Where(a => a.Version == req.Version);
        }

        var count = await query.ExecuteUpdateAsync(updateBuilder, ct).ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.Conflict();
        }

        return TypedResults.Ok(new Response(newVersion));
    }
}
