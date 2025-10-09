namespace WebApp.Api.Common.Http;

public interface IOffsetPagination
{
    int Page { get; }
    int Size { get; }
}
