using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Common.Projection;

public sealed class ProjectableJsonConverter(IProjectionService projectionService)
    : JsonConverter<Projectable>
{
    public override Projectable Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotSupportedException("Deserialization of Projectable is not supported.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        Projectable value,
        JsonSerializerOptions options
    )
    {
        if (string.IsNullOrEmpty(value.Fields))
        {
            JsonSerializer.Serialize(writer, value.Value, value.Value.GetType(), options);
        }
        else
        {
            projectionService.Project(writer, value.Value, value.Fields, options);
        }
    }
}
