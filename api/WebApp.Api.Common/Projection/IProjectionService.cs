using System.Linq.Expressions;
using System.Text.Json;

namespace WebApp.Api.Common.Projection;

public interface IProjectionService
{
    IDictionary<string, object?> Project<T>(T instance, string fields)
        where T : notnull;
    void Project(
        Utf8JsonWriter writer,
        object instance,
        string fields,
        JsonSerializerOptions options
    );
    Expression<Func<T, T>> Project<T>(string fields);
}
