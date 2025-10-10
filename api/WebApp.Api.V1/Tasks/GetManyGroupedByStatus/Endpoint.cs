using System.Linq.Expressions;
using System.Reflection;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.GetManyGroupedByStatus;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Ok<List<Paginated<Projectable>>>>
{
    public override void Configure()
    {
        Get("tasks/group-by/status");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Ok<List<Paginated<Projectable>>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Tasks.Where(a => a.DeletedTime == null);
        if (req.ProjectId.HasValue)
        {
            query = query.Where(a => a.ProjectId == req.ProjectId && a.Project.DeletedTime == null);
        }

        var pagination = OffsetPagination.From(req);
        var order = Orderable.From(req);
        var groups = await query
            .GroupBy(a => a.StatusId ?? new StatusId(-1))
            .Select(BuildGroupedSelectExpression(pagination, order, req.Fields))
            .ToListAsync(ct)
            .ConfigureAwait(false);
        return TypedResults.Ok(
            groups
                .Select(wrapped =>
                    Paginated.From(
                        wrapped.Items.Select(task => task.ToProjectable(req.Fields)),
                        pagination,
                        wrapped.TotalCount
                    )
                )
                .ToList()
        );
    }

    public Expression<Func<IGrouping<StatusId, TaskEntity>, Wrapped>> BuildGroupedSelectExpression(
        OffsetPagination pagination,
        Orderable order,
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

        var skipMethod = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Queryable.Skip) && a.GetParameters().Length == 2)
            .MakeGenericMethod(itemType);
        var skipCall = Expression.Call(
            skipMethod,
            asQueryExpr,
            Expression.Constant(pagination.Offset)
        );

        var takeMethod = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Queryable.Take) && a.GetParameters().Length == 2)
            .MakeGenericMethod(itemType);
        var takeCall = Expression.Call(takeMethod, skipCall, Expression.Constant(pagination.Size));

        var orderByCall = BuildOrderByExpression(takeCall, itemParam, order.Sort);

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
        var selectCall = Expression.Call(selectMethod, orderByCall, itemLambdaExpr);
        var toListMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Enumerable.ToList) && a.GetParameters().Length == 1)
            .MakeGenericMethod(itemType);
        var toListCall = Expression.Call(toListMethod, selectCall);
        var itemsBindExpr = Expression.Bind(
            typeof(Wrapped).GetProperty(nameof(Wrapped.Items))!,
            toListCall
        );
        var countMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(a => a.Name == nameof(Enumerable.Count) && a.GetParameters().Length == 1)
            .MakeGenericMethod(itemType);
        var totalCountBindExpr = Expression.Bind(
            typeof(Wrapped).GetProperty(nameof(Wrapped.TotalCount))!,
            Expression.Call(countMethod, groupParam)
        );

        var groupInitExpr = Expression.MemberInit(
            Expression.New(typeof(Wrapped)),
            itemsBindExpr,
            totalCountBindExpr
        );
        var groupLambdaExpr = Expression.Lambda<Func<IGrouping<StatusId, TaskEntity>, Wrapped>>(
            groupInitExpr,
            groupParam
        );

        return groupLambdaExpr;
    }

    public static Expression BuildOrderByExpression(
        Expression sourceExpr,
        ParameterExpression parameter,
        string sortString
    )
    {
        var fields = sortString.Split(
            ',',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );

        Expression resultExpr = sourceExpr;
        bool isFirst = true;
        var queryableType = typeof(Queryable);

        foreach (var field in fields)
        {
            bool descending = field.StartsWith("-");
            var fieldName = descending ? field[1..].Trim() : field.Trim();

            var member = BuildPropertyAccess(parameter, fieldName);
            var lambdaType = typeof(Func<,>).MakeGenericType(parameter.Type, member.Type);
            var lambda = Expression.Lambda(lambdaType, member, parameter);

            string methodName;
            if (isFirst)
                methodName = descending
                    ? nameof(Queryable.OrderByDescending)
                    : nameof(Queryable.OrderBy);
            else
                methodName = descending
                    ? nameof(Queryable.ThenByDescending)
                    : nameof(Queryable.ThenBy);

            var method = queryableType
                .GetMethods()
                .Where(a => a.Name == methodName && a.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(parameter.Type, member.Type);

            resultExpr = Expression.Call(method, resultExpr, lambda);
            isFirst = false;
        }

        return resultExpr;
    }

    private static MemberExpression BuildPropertyAccess(ParameterExpression parameter, string field)
    {
        var segments = field.Split('.');
        MemberExpression? member = null;
        foreach (var segment in segments)
        {
            member = member is null
                ? Expression.PropertyOrField(parameter, segment)
                : Expression.PropertyOrField(member, segment);
        }
        return member!;
    }

    public sealed record Wrapped
    {
        public required List<TaskEntity> Items { get; init; }
        public required int TotalCount { get; init; }
    }
}
