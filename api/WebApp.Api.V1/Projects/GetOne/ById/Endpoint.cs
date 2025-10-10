using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Projects.GetOne.ById;

public sealed class Endpoint(AppDbContext db, IProjectionService projectionService)
    : Endpoint<Request, Results<NotFound, Ok<Projectable>>>
{
    public override void Configure()
    {
        Get("projects/{ProjectId}");
        Version(1);
    }

    public override async Task<Results<NotFound, Ok<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Projects.Where(a => a.DeletedTime == null && a.Id == req.ProjectId);

        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(projectionService.Project<Project>(req.Fields));
        }

        var project = await query.FirstOrDefaultAsync(ct).ConfigureAwait(false);
        if (project is null)
        {
            return TypedResults.NotFound();
        }
        else
        {
            return TypedResults.Ok(project.ToProjectable(req.Fields));
        }
    }
}
