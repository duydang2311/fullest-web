using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.TaskAssignees.Batch;

public sealed class Endpoint(AppDbContext db, IEventHub eventHub)
    : Endpoint<
        Request,
        Results<BadRequest<Problem>, NotFound<Problem>, Conflict<Problem>, NoContent>
    >
{
    public override void Configure()
    {
        Post("task-assignees/batch");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<
        Results<BadRequest<Problem>, NotFound<Problem>, Conflict<Problem>, NoContent>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        Guard.Against.Null(req.TaskId);

        await using var tx = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        try
        {
            var taskId = req.TaskId.Value;
            var requestedAssigned = req.Assigned?.Distinct().ToArray();
            var requestedUnassigned = req.Unassigned?.Distinct().ToArray();

            var existingAssignees = await db
                .TaskAssignees.Where(a => a.TaskId == taskId)
                .Select(a => a.UserId)
                .ToHashSetAsync(ct)
                .ConfigureAwait(false);
            var currentAssignees = existingAssignees.ToHashSet();

            if (existingAssignees.Count == 0)
            {
                var taskExists = await db
                    .Tasks.AnyAsync(t => t.Id == taskId, ct)
                    .ConfigureAwait(false);
                if (!taskExists)
                {
                    return TypedResults.NotFound(
                        Problem.FromError(nameof(Request.TaskId), ErrorCodes.NotFound)
                    );
                }
            }

            var assignedUserIds = requestedAssigned is null
                ? []
                : requestedAssigned.Where(userId => !currentAssignees.Contains(userId)).ToArray();
            var unassignedUserIds = requestedUnassigned is null
                ? []
                : requestedUnassigned.Where(userId => currentAssignees.Contains(userId)).ToArray();

            if (assignedUserIds.Length > 0)
            {
                await db.AddRangeAsync(
                        assignedUserIds.Select(userId => new TaskAssignee
                        {
                            TaskId = taskId,
                            UserId = userId,
                        }),
                        ct
                    )
                    .ConfigureAwait(false);
            }

            if (unassignedUserIds.Length > 0)
            {
                await db
                    .TaskAssignees.Where(a =>
                        a.TaskId == taskId && unassignedUserIds.Contains(a.UserId)
                    )
                    .ExecuteDeleteAsync(ct)
                    .ConfigureAwait(false);
            }

            await db.SaveChangesAsync(ct).ConfigureAwait(false);

            var projectId = (ProjectId)HttpContext.Items["ProjectId"]!;
            Console.WriteLine(
                "Publish: "
                    + new TaskUpdated(
                        projectId,
                        taskId,
                        [
                            .. assignedUserIds.Select(userId => new TaskAssigned(
                                projectId,
                                taskId,
                                req.CallerId,
                                userId
                            )),
                            .. unassignedUserIds.Select(userId => new TaskUnassigned(
                                projectId,
                                taskId,
                                req.CallerId,
                                userId
                            )),
                        ]
                    )
            );
            if (assignedUserIds.Length > 0 || unassignedUserIds.Length > 0)
            {
                await eventHub
                    .PublishAsync(
                        new TaskUpdated(
                            projectId,
                            taskId,
                            [
                                .. assignedUserIds.Select(userId => new TaskAssigned(
                                    projectId,
                                    taskId,
                                    req.CallerId,
                                    userId
                                )),
                                .. unassignedUserIds.Select(userId => new TaskUnassigned(
                                    projectId,
                                    taskId,
                                    req.CallerId,
                                    userId
                                )),
                            ]
                        ),
                        ct
                    )
                    .ConfigureAwait(false);
            }
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
            await tx.CommitAsync(ct).ConfigureAwait(false);
            return TypedResults.NoContent();
        }
        catch (ReferenceConstraintException e)
            when (e.ConstraintProperties.Any(a =>
                    a.Equals(nameof(TaskAssignee.UserId), StringComparison.Ordinal)
                )
            )
        {
            return TypedResults.BadRequest(
                Problem.FromError(nameof(Request.Assigned), ErrorCodes.NotFound)
            );
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
        catch (UniqueConstraintException)
        {
            return TypedResults.Conflict(Problem.FromError(ErrorCodes.Conflict));
        }
    }
}
