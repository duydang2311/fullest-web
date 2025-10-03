using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Api.Common.Codecs;
using WebApp.Domain.Entities;

namespace WebApp.Api.Serialization;

public class EntityIdJsonConverter<T>(INumberEncoder numberEncoder) : JsonConverter<T>
    where T : IEntityId<long>, new()
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token for UserId.");
        }
        var input = reader.GetString() ?? throw new JsonException("UserId cannot be null.");
        if (!numberEncoder.TryDecode(input, out long id))
        {
            throw new JsonException("Invalid entity id format.");
        }
        return new T { Value = id };
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(numberEncoder.Encode(value.Value));
    }
}
