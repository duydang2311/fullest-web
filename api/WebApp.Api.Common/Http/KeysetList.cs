namespace WebApp.Api.Common.Http;

public sealed record KeysetList<T, TKey>(
    IEnumerable<T> Items,
    bool HasPrevious,
    bool HasNext,
    TKey? Before,
    TKey? After,
    int TotalCount
) : IPaginatedList<T>;

public static class KeysetList
{
    public static KeysetList<T, TKey> From<T, TKey>(
        IEnumerable<T> items,
        bool hasPrevious,
        bool hasNext,
        TKey? before,
        TKey? after,
        int totalCount
    ) => new(items, hasPrevious, hasNext, before, after, totalCount);
}
