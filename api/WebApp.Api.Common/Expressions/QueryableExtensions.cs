using System.Linq.Expressions;
using WebApp.Api.Common.Http;

namespace System.Linq;

public static class QueryableExtensions
{
    public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(
        this IQueryable<TSource> source,
        Direction direction,
        Expression<Func<TSource, TKey>> keySelector
    )
    {
        return direction.IsAscending
            ? source.OrderBy(keySelector)
            : source.OrderByDescending(keySelector);
    }
}
