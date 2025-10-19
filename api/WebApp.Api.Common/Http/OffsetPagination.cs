namespace WebApp.Api.Common.Http
{
    public sealed record OffsetPagination : IOffsetPagination
    {
        private readonly int page = 1;
        private readonly int size = 20;

        public int Page
        {
            get => page;
            init => page = Math.Max(1, value);
        }

        public int Size
        {
            get => size;
            init => size = Math.Clamp(value, 1, 100);
        }

        public int Offset => (Page - 1) * Size;

        public static OffsetPagination From(IOffsetPagination pagination) =>
            new() { Page = pagination.Page, Size = pagination.Size };
    }
}

namespace System.Linq
{
    using WebApp.Api.Common.Http;

    public static class OffsetPaginationExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int size)
        {
            return query.Skip((page - 1) * size).Take(size);
        }

        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> query,
            IOffsetPagination pagination
        )
        {
            return query.Paginate(pagination.Page, pagination.Size);
        }

        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> query,
            OffsetPagination pagination
        )
        {
            return query.Skip(pagination.Offset).Take(pagination.Size);
        }

        public static IQueryable<T> Paginate<T>(
            this OffsetPagination pagination,
            IQueryable<T> query
        ) => query.Skip(pagination.Offset).Take(pagination.Size);

        public static IEnumerable<T> Paginate<T>(
            this IEnumerable<T> enumerable,
            OffsetPagination pagination
        )
        {
            return enumerable.Skip(pagination.Offset).Take(pagination.Size);
        }
    }
}
