using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Api.Common.Codecs;
using WebApp.Domain.Entities;

namespace WebApp.Api.Serialization;

public class UserIdJsonConverter(INumberEncoder numberEncoder) : JsonConverter<UserId>
{
    public override UserId Read(
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
        if (!numberEncoder.TryDecode(input, out long userId))
        {
            throw new JsonException("Invalid UserId format.");
        }
        return new UserId(userId);
    }

    public override void Write(Utf8JsonWriter writer, UserId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(numberEncoder.Encode(value.Value));
    }
}
