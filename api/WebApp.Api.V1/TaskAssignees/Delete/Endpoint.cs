using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.TaskAssignees.Delete;

public sealed class Endpoint(AppDbContext db, IEventHub eventHub)
    : Endpoint<Request, Results<NotFound<Problem>, NoContent>>
{
    public override void Configure()
    {
        Delete("tasks/{TaskId}/assignees/{UserId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound<Problem>, NoContent>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        var count = await db
            .TaskAssignees.Where(a => a.TaskId == req.TaskId && a.UserId == req.UserId)
            .ExecuteDeleteAsync(ct)
            .ConfigureAwait(false);

        if (count == 0)
        {
            return TypedResults.NotFound(Problem.FromError(ErrorCodes.NotFound));
        }

        var projectId = (ProjectId)HttpContext.Items["ProjectId"]!;
        await eventHub
            .PublishAsync(new TaskUnassigned(projectId, req.TaskId, req.CallerId, req.UserId), ct)
            .ConfigureAwait(false);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        await tx.CommitAsync(ct).ConfigureAwait(false);

        return TypedResults.NoContent();
    }
}
