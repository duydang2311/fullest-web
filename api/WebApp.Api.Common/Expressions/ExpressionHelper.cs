using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace WebApp.Api.Common.Expressions;

public static class ExpressionHelper
{
    public static Expression<Func<T, T>> Append<T>(
        Expression<Func<T, T>>? left,
        Expression<Func<T, T>> right
    )
    {
        if (left is null)
        {
            return right;
        }

        var replace = new ReplacingExpressionVisitor(right.Parameters, [left.Body]);
        var combined = replace.Visit(right.Body);
        return Expression.Lambda<Func<T, T>>(combined, left.Parameters);
    }
}
