namespace WebApp.Api.Common.Http;

public interface IPaginatedList<T>
{
    IEnumerable<T> Items { get; }
    int TotalCount { get; }
}
