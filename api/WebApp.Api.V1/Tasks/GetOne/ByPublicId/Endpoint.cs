using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.GetOne.ByPublicId;

public sealed class Endpoint(AppDbContext db, IProjectionService projectionService)
    : Endpoint<Request, Results<NotFound, Ok<Projectable>>>
{
    public override void Configure()
    {
        Get("projects/{ProjectId}/tasks/{PublicId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Ok<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Tasks.Where(a =>
            a.DeletedTime == null && a.PublicId == req.PublicId && a.ProjectId == req.ProjectId
        );
        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(projectionService.Project<TaskEntity>(req.Fields));
        }
        var task = await query.FirstOrDefaultAsync(ct).ConfigureAwait(false);
        if (task is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(task.ToProjectable(req.Fields));
    }
}
