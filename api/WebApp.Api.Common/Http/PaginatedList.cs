namespace WebApp.Api.Common.Http;

public sealed record PaginatedList<T>(IEnumerable<T> Items, int Page, int Size, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / Size);
}

public static class PaginatedList
{
    public static PaginatedList<T> From<T>(
        IEnumerable<T> items,
        int page,
        int size,
        int totalItems
    ) => new(items, page, size, totalItems);

    public static PaginatedList<T> From<T>(
        IEnumerable<T> items,
        IOffsetPagination pagination,
        int totalCount
    ) => new(items, pagination.Page, pagination.Size, totalCount);
}
