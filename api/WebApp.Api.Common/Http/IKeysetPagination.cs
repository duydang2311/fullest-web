namespace WebApp.Api.Common.Http;

public interface IKeysetPagination<T>
{
    T After { get; }
    T Before { get; }
    int Size { get; }
}
