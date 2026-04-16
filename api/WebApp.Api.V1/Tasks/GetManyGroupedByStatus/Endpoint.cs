using System.Linq.Expressions;
using System.Reflection;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.GetManyGroupedByStatus;

public sealed class Endpoint(AppDbContext db, INumberEncoder numberEncoder)
    : Endpoint<Request, Ok<Dictionary<string, KeysetList<Projectable>>>>
{
    public override void Configure()
    {
        Get("tasks/grouped/status");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Ok<Dictionary<string, KeysetList<Projectable>>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Tasks.Where(a => a.DeletedTime == null);
        if (req.ProjectId.HasValue)
        {
            query = query.Where(a => a.ProjectId == req.ProjectId && a.Project.DeletedTime == null);
        }

        var countGroups = await query
            .GroupBy(a => a.StatusId ?? new StatusId(-1))
            .Select(g => g.Count())
            .ToListAsync()
            .ConfigureAwait(false);
        var groups = await query
            .GroupBy(a => a.StatusId ?? new StatusId(-1))
            .Select(
                BuildGroupedSelectExpression(
                    req.Direction,
                    req.Size,
                    req.IncludeTotalCount,
                    req.Select
                )
            )
            .ToDictionaryAsync(a => a.Key, ct)
            .ConfigureAwait(false);
        return TypedResults.Ok(
            groups.ToDictionary(
                kvp => kvp.Key.Value == -1 ? "none" : numberEncoder.Encode(kvp.Key.Value),
                kvp =>
                {
                    var hasNext = kvp.Value.Items.Count > req.Size;
                    if (hasNext)
                    {
                        kvp.Value.Items.RemoveAt(kvp.Value.Items.Count - 1);
                    }
                    return KeysetList.From(
                        items: kvp.Value.Items.Select(Projectable.From<TaskEntity>(req.Select)),
                        hasPrevious: false,
                        hasNext: hasNext,
                        kvp.Value.TotalCount
                    );
                }
            )
        );
    }

    private Expression<
        Func<IGrouping<StatusId, TaskEntity>, GroupedList>
    > BuildGroupedSelectExpression(
        Direction direction,
        int size,
        bool includeTotalCount,
        string? fields
    )
    {
        var groupType = typeof(IGrouping<StatusId, TaskEntity>);
        var itemType = typeof(TaskEntity);

        var groupParam = Expression.Parameter(groupType);
        var itemParam = Expression.Parameter(itemType);

        var asQueryExpr = Expression.Call(
            typeof(Queryable),
            nameof(Queryable.AsQueryable),
            [itemType],
            groupParam
        );

        Expression orderCall = null!;
        MethodInfo orderMethod = null!;
        if (direction.IsAscending)
        {
            orderMethod = typeof(Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(a => a.Name == nameof(Queryable.OrderBy) && a.GetParameters().Length == 2)
                .MakeGenericMethod(itemType, typeof(TaskId));
        }
        else
        {
            orderMethod = typeof(Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(a =>
                    a.Name == nameof(Queryable.OrderByDescending) && a.GetParameters().Length == 2
                )
                .MakeGenericMethod(itemType, typeof(TaskId));
        }
        var param = Expression.Parameter(typeof(TaskEntity));
        var lambdaType = typeof(Func<,>).MakeGenericType(typeof(TaskEntity), typeof(TaskId));
        var lambda = Expression.Lambda(
            lambdaType,
            Expression.Property(param, nameof(TaskEntity.Id)),
            param
        );
        orderCall = Expression.Call(orderMethod, asQueryExpr, lambda);

        var takeMethod = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Queryable.Take) && a.GetParameters().Length == 2)
            .MakeGenericMethod(itemType);
        var takeCall = Expression.Call(takeMethod, orderCall, Expression.Constant(size + 1));

        var itemLambdaExpr = Expression.Lambda<Func<TaskEntity, TaskEntity>>(
            string.IsNullOrEmpty(fields)
                ? itemParam
                : FieldProjector.BuildMemberInitExpression(
                    itemType,
                    itemParam,
                    FieldProjector.BuildFieldTreeSpan(fields ?? "")
                ),
            itemParam
        );
        var selectMethod = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Queryable.Select) && a.GetParameters().Length == 2)
            .MakeGenericMethod(itemType, itemType);
        var selectCall = Expression.Call(selectMethod, takeCall, itemLambdaExpr);
        var toListMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Enumerable.ToList) && a.GetParameters().Length == 1)
            .MakeGenericMethod(itemType);
        var toListCall = Expression.Call(toListMethod, selectCall);
        var itemsBindExpr = Expression.Bind(
            typeof(GroupedList).GetProperty(nameof(GroupedList.Items))!,
            toListCall
        );
        var keyProperty = typeof(IGrouping<StatusId, TaskEntity>).GetProperty(
            nameof(IGrouping<,>.Key)
        )!;
        var keyBindExpr = Expression.Bind(
            typeof(GroupedList).GetProperty(nameof(GroupedList.Key))!,
            Expression.Property(groupParam, keyProperty)
        );

        var bindingExprs = new List<MemberBinding>([itemsBindExpr, keyBindExpr]);
        if (includeTotalCount)
        {
            var countMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(a => a.Name == nameof(Enumerable.Count) && a.GetParameters().Length == 1)
                .MakeGenericMethod(itemType);
            var totalCountBindExpr = Expression.Bind(
                typeof(GroupedList).GetProperty(nameof(GroupedList.TotalCount))!,
                Expression.Call(countMethod, groupParam)
            );
            bindingExprs.Add(totalCountBindExpr);
        }
        var groupInitExpr = Expression.MemberInit(
            Expression.New(typeof(GroupedList)),
            bindingExprs
        );
        var groupLambdaExpr = Expression.Lambda<Func<IGrouping<StatusId, TaskEntity>, GroupedList>>(
            groupInitExpr,
            groupParam
        );

        return groupLambdaExpr;
    }

    private sealed record GroupedList
    {
        public required StatusId Key { get; init; }
        public required List<TaskEntity> Items { get; init; }
        public int TotalCount { get; init; }
    }
}
