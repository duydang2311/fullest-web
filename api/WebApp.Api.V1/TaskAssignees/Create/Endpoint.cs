using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.TaskAssignees.Create;

public sealed class Endpoint(AppDbContext db, IEventHub eventHub)
    : Endpoint<
        Request,
        Results<BadRequest<Problem>, NotFound<Problem>, Conflict<Problem>, Created<Response>>
    >
{
    public override void Configure()
    {
        Post("task-assignees");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<
        Results<BadRequest<Problem>, NotFound<Problem>, Conflict<Problem>, Created<Response>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        Guard.Against.Null(req.TaskId);
        Guard.Against.Null(req.UserId);

        await using var tx = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        var assignee = new TaskAssignee { TaskId = req.TaskId.Value, UserId = req.UserId.Value };

        try
        {
            await db.AddAsync(assignee, ct).ConfigureAwait(false);
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (UniqueConstraintException)
        {
            return TypedResults.Conflict(Problem.FromError(ErrorCodes.Conflict));
        }
        catch (ReferenceConstraintException e)
            when (e.ConstraintProperties.Any(a =>
                    a.Equals(nameof(TaskAssignee.TaskId), StringComparison.Ordinal)
                )
            )
        {
            return TypedResults.NotFound(
                Problem.FromError(nameof(Request.TaskId), ErrorCodes.NotFound)
            );
        }
        catch (ReferenceConstraintException e)
            when (e.ConstraintProperties.Any(a =>
                    a.Equals(nameof(TaskAssignee.UserId), StringComparison.Ordinal)
                )
            )
        {
            return TypedResults.BadRequest(
                Problem.FromError(nameof(Request.UserId), ErrorCodes.NotFound)
            );
        }

        await eventHub
            .PublishAsync(
                new TaskUpdated(
                    req.TaskId.Value,
                    [new TaskAssigned(req.TaskId.Value, req.CallerId, req.UserId.Value)]
                ),
                ct
            )
            .ConfigureAwait(false);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        await tx.CommitAsync(ct).ConfigureAwait(false);

        return TypedResults.Created(string.Empty, new Response(assignee.TaskId, assignee.UserId));
    }
}
