namespace WebApp.Api.Common.Http;

public sealed record CursorList<T, TKey>(IEnumerable<T> Items, TKey Cursor, bool HasMore);

public static class CursorList
{
    public static CursorList<T, TKey> From<T, TKey>(
        IEnumerable<T> items,
        TKey cursor,
        bool hasMore
    ) => new(items, cursor, hasMore);
}
