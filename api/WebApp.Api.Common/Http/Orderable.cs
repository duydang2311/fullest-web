using System.Linq.Expressions;

namespace WebApp.Api.Common.Http;

public sealed record Orderable : IOrderable
{
    public string Sort { get; init; } = string.Empty;

    public static Orderable From(IOrderable orderable) => new() { Sort = orderable.Sort };
}

public static class OrderableExtensions
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> query, IOrderable orderable)
    {
        var fields = orderable.Sort.Split(
            ',',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        var isOrdered = query.Expression.Type == typeof(IOrderedQueryable<T>);
        var parameter = Expression.Parameter(typeof(T));
        foreach (var field in fields)
        {
            if (field[0] == '-')
            {
                var body = BuildPropertyAccess(parameter, field[1..].Trim());
                query = isOrdered
                    ? ((IOrderedQueryable<T>)query).ThenByDescending(
                        BuildOrderLambda<T>(parameter, body)
                    )
                    : query.OrderByDescending(BuildOrderLambda<T>(parameter, body));
            }
            else
            {
                var body = BuildPropertyAccess(parameter, field);
                query = isOrdered
                    ? ((IOrderedQueryable<T>)query).ThenBy(BuildOrderLambda<T>(parameter, body))
                    : query.OrderBy(BuildOrderLambda<T>(parameter, body));
            }
            isOrdered = true;
        }
        return query;
    }

    public static IQueryable<T> Sort<T>(this IOrderable orderable, IQueryable<T> query)
    {
        return query.Sort(orderable);
    }

    public static IQueryable<T> SortOrDefault<T>(
        this IQueryable<T> query,
        IOrderable orderable,
        Func<IQueryable<T>, IQueryable<T>> fn
    )
    {
        return string.IsNullOrEmpty(orderable.Sort) ? fn(query) : query.Sort(orderable);
    }

    private static MemberExpression BuildPropertyAccess(ParameterExpression parameter, string field)
    {
        // TODO: support accessing collection property
        var segments = field.Split('.');
        MemberExpression? member = null;
        foreach (var segment in segments)
        {
            member = member is null
                ? Expression.Property(parameter, segment)
                : Expression.Property(member, segment);
        }
        return member!;
    }

    private static Expression<Func<T, object>> BuildOrderLambda<T>(
        ParameterExpression parameter,
        MemberExpression body
    )
    {
        return Expression.Lambda<Func<T, object>>(
            Expression.Convert(body, typeof(object)),
            parameter
        );
    }
}
