namespace WebApp.Api.Common.Http;

public sealed record OffsetList<T>(IEnumerable<T> Items, int Page, int Size, int TotalCount)
    : IPaginatedList<T>
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Size);
}

public static class OffsetList
{
    public static OffsetList<T> From<T>(IEnumerable<T> items, int page, int size, int totalItems) =>
        new(items, page, size, totalItems);

    public static OffsetList<T> From<T>(
        IEnumerable<T> items,
        IOffsetPagination pagination,
        int totalCount
    ) => new(items, pagination.Page, pagination.Size, totalCount);
}
