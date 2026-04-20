using System.Data;
using System.Linq.Expressions;
using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NodaTime;
using WebApp.Application.Common;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.Patch;

public sealed class Endpoint(AppDbContext db, IEventHub eventHub)
    : Endpoint<Request, Results<NotFound, Conflict, Ok<Response>>>
{
    public override void Configure()
    {
        Patch("tasks/{TaskId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, Conflict, Ok<Response>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        Guard.Against.Null(req.Patch);

        var projectId = (ProjectId)HttpContext.Items["ProjectId"]!;
        var mappings = new List<string>();
        var changes = new List<TaskPropertyChanged>();
        Action<UpdateSettersBuilder<TaskEntity>>? updateBuilder = null;

        await using var tx = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

        if (req.Patch.TryGetValue(a => a.Title, out var title))
        {
            updateBuilder += a => a.SetProperty(b => b.Title, title);
            mappings.Add(nameof(TaskEntity.Title));
        }
        if (req.Patch.TryGetValue(a => a.StatusId, out var statusId))
        {
            updateBuilder += a => a.SetProperty(b => b.StatusId, statusId);
            mappings.Add(nameof(TaskEntity.StatusId));
        }
        if (req.Patch.TryGetValue(a => a.PriorityId, out var priorityId))
        {
            updateBuilder += a => a.SetProperty(b => b.PriorityId, priorityId);
            mappings.Add(nameof(TaskEntity.PriorityId));
        }
        if (req.Patch.TryGetValue(a => a.DueTime, out var dueTime))
        {
            updateBuilder += a => a.SetProperty(b => b.DueTime, dueTime);
            mappings.Add(nameof(TaskEntity.DueTime));
        }
        if (req.Patch.TryGetValue(a => a.DueTz, out var dueTz))
        {
            updateBuilder += a => a.SetProperty(b => b.DueTz, dueTz);
            mappings.Add(nameof(TaskEntity.DueTz));
        }
        if (req.Patch.TryGetValue(a => a.DescriptionJson, out var descriptionJson))
        {
            var (json, preview) = TextDocumentHelper.ParseDocumentPreview(descriptionJson);
            updateBuilder += a =>
                a.SetProperty(b => b.DescriptionJson, json)
                    .SetProperty(b => b.DescriptionPreview, preview);
            mappings.Add(nameof(TaskEntity.DescriptionJson));
        }

        Guard.Against.Null(updateBuilder);
        var newVersion = req.Version + 1;
        updateBuilder += a => a.SetProperty(b => b.Version, newVersion);

        mappings.Add(nameof(TaskEntity.Version));
        var snapshot = await db
            .Tasks.Where(a => a.DeletedTime == null && a.Id == req.TaskId)
            .Select(BuildSnapshotSelector<TaskEntity, TaskSnapshot>(mappings))
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (snapshot is null)
        {
            return TypedResults.NotFound();
        }
        if (snapshot.Version != req.Version)
        {
            return TypedResults.Conflict();
        }

        if (req.Patch.Has(a => a.StatusId))
        {
            changes.Add(
                new TaskStatusChanged(
                    projectId,
                    req.TaskId,
                    req.CallerId,
                    req.Patch.StatusId,
                    snapshot.StatusId
                )
            );
        }
        if (req.Patch.Has(a => a.PriorityId))
        {
            changes.Add(
                new TaskPriorityChanged(
                    projectId,
                    req.TaskId,
                    req.CallerId,
                    req.Patch.PriorityId,
                    snapshot.PriorityId
                )
            );
        }
        if (req.Patch.Has(a => a.DueTime) || req.Patch.Has(a => a.DueTz))
        {
            changes.Add(
                new TaskDueTimeChanged(
                    projectId,
                    req.TaskId,
                    req.CallerId,
                    req.Patch.DueTime,
                    req.Patch.DueTz,
                    snapshot.DueTime,
                    snapshot.DueTz
                )
            );
        }
        if (req.Patch.Has(a => a.Title))
        {
            changes.Add(
                new TaskTitleChanged(
                    projectId,
                    req.TaskId,
                    req.CallerId,
                    Guard.Against.Null(req.Patch.Title),
                    Guard.Against.Null(snapshot.Title)
                )
            );
        }
        if (req.Patch.Has(a => a.DescriptionJson))
        {
            changes.Add(
                new TaskDescriptionChanged(
                    projectId,
                    req.TaskId,
                    req.CallerId,
                    req.Patch.DescriptionJson,
                    snapshot.DescriptionJson
                )
            );
        }

        var query = db.Tasks.Where(a =>
            a.Id == req.TaskId && a.DeletedTime == null && a.Version == req.Version
        );
        var count = await query.ExecuteUpdateAsync(updateBuilder, ct).ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.Conflict();
        }

        if (changes.Count > 0)
        {
            await eventHub
                .PublishAsync(new TaskUpdated(projectId, req.TaskId, changes), ct)
                .ConfigureAwait(false);
        }
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        await tx.CommitAsync(ct).ConfigureAwait(false);

        return TypedResults.Ok(new Response(newVersion));
    }

    private sealed record TaskSnapshot
    {
        public string? Title { get; init; }
        public StatusId? StatusId { get; init; }
        public PriorityId? PriorityId { get; init; }
        public Instant? DueTime { get; init; }
        public string? DueTz { get; init; }
        public string? DescriptionJson { get; init; }
        public uint Version { get; init; }
    }

    private static Expression<Func<TEntity, TSnapshot>> BuildSnapshotSelector<TEntity, TSnapshot>(
        IEnumerable<string> propertyNames
    )
        where TSnapshot : new()
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var bindings = propertyNames.Select(name =>
            Expression.Bind(
                typeof(TSnapshot).GetProperty(name)!,
                Expression.Property(parameter, name)
            )
        );
        var newExpr = Expression.New(typeof(TSnapshot));
        var memberInit = Expression.MemberInit(newExpr, bindings);

        return Expression.Lambda<Func<TEntity, TSnapshot>>(memberInit, parameter);
    }
}
