namespace WebApp.Api.Common.Http;

public sealed record Paginated<T>(IEnumerable<T> Items, int Page, int Size, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / Size);
}

public static class Paginated
{
    public static Paginated<T> From<T>(IEnumerable<T> items, int page, int size, int totalItems) =>
        new(items, page, size, totalItems);

    public static Paginated<T> From<T>(
        IEnumerable<T> items,
        OffsetPagination pagination,
        int totalCount
    ) => new(items, pagination.Page, pagination.Size, totalCount);
}
