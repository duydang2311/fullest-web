using System.Linq.Expressions;
using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApp.Api.Common.Expressions;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.Patch;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Results<NotFound, NoContent>>
{
    public override void Configure()
    {
        Patch("tasks/{TaskId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, NoContent>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        Guard.Against.Null(req.Patch);

        var query = db.Tasks.Where(a => a.Id == req.TaskId && a.DeletedTime == null);

        Expression<
            Func<SetPropertyCalls<TaskEntity>, SetPropertyCalls<TaskEntity>>
        >? setPropertyCalls = null;

        if (req.Patch.TryGetValue(a => a.Title, out var title))
        {
            setPropertyCalls = ExpressionHelper.Append(
                setPropertyCalls,
                a => a.SetProperty(b => b.Title, title!)
            );
        }

        if (req.Patch.TryGetValue(a => a.StatusId, out var statusId))
        {
            setPropertyCalls = ExpressionHelper.Append(
                setPropertyCalls,
                a => a.SetProperty(b => b.StatusId, statusId)
            );
        }

        if (req.Patch.TryGetValue(a => a.PriorityId, out var priorityId))
        {
            setPropertyCalls = ExpressionHelper.Append(
                setPropertyCalls,
                a => a.SetProperty(b => b.PriorityId, priorityId)
            );
        }

        if (req.Patch.TryGetValue(a => a.DueTime, out var dueTime))
        {
            setPropertyCalls = ExpressionHelper.Append(
                setPropertyCalls,
                a => a.SetProperty(b => b.DueTime, dueTime)
            );
        }

        if (req.Patch.TryGetValue(a => a.DueTz, out var dueTz))
        {
            setPropertyCalls = ExpressionHelper.Append(
                setPropertyCalls,
                a => a.SetProperty(b => b.DueTz, dueTz)
            );
        }

        Guard.Against.Null(setPropertyCalls);

        var count = await query.ExecuteUpdateAsync(setPropertyCalls, ct).ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}
