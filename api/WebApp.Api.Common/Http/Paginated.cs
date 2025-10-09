namespace WebApp.Api.Common.Http;

public sealed record Paginated<T>(IEnumerable<T> Items, int Page, int Size, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Size);
}

public static class Paginated
{
    public static Paginated<T> From<T>(IEnumerable<T> items, int page, int size, int totalCount) =>
        new(items, page, size, totalCount);

    public static Paginated<T> From<T>(
        IEnumerable<T> items,
        OffsetPagination pagination,
        int totalCount
    ) => new(items, pagination.Page, pagination.Size, totalCount);
}
