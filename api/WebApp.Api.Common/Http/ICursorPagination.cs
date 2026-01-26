namespace WebApp.Api.Common.Http;

public interface ICursorPagination<T>
{
    T Cursor { get; }
    int Size { get; }
}
