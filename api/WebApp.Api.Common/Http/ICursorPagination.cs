namespace WebApp.Api.Common.Http;

public interface ICursorPagination<T>
{
    T After { get; }
    int Size { get; }
}
