namespace WebApp.Api.Common.Http;

public sealed record KeysetList<T>(
    IEnumerable<T> Items,
    bool HasPrevious,
    bool HasNext,
    int TotalCount
);

public static class KeysetList
{
    public static KeysetList<T> From<T>(
        IEnumerable<T> items,
        bool hasPrevious,
        bool hasNext,
        int totalCount = 0
    ) => new(items, hasPrevious, hasNext, totalCount);
}
