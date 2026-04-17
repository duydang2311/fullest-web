using System.Text;
using System.Text.Json;

namespace WebApp.Application.Common;

public static class TextDocumentHelper
{
    public static (string? Json, string? Preview) ParseDocumentPreview(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return (null, null);
        }

        try
        {
            using var doc = JsonSerializer.Deserialize<JsonDocument>(json);
            if (doc is null || doc.RootElement.ValueKind == JsonValueKind.Null)
            {
                return (null, null);
            }
            return (JsonSerializer.Serialize(doc), ExtractText(doc, 256));
        }
        catch
        {
            return (null, null);
        }
    }

    private static string? ExtractText(JsonDocument doc, int size)
    {
        var stack = new Stack<JsonElement>();
        var sb = new StringBuilder(size);
        stack.Push(doc.RootElement);
        while (sb.Length < size && stack.TryPop(out var el))
        {
            if (!el.TryGetProperty("type", out var type))
            {
                continue;
            }

            var value = type.GetString();
            if (string.Equals(value, "text", StringComparison.OrdinalIgnoreCase))
            {
                if (el.TryGetProperty("text", out var text))
                {
                    var trimmed = text.GetString()?.Trim();
                    if (sb.Length + (trimmed?.Length ?? 0) >= size)
                    {
                        break;
                    }
                    sb.Append(trimmed).Append(' ');
                }
                continue;
            }

            if (
                el.TryGetProperty("content", out var content)
                && content.ValueKind == JsonValueKind.Array
            )
            {
                foreach (var childEl in content.EnumerateArray())
                {
                    stack.Push(childEl);
                }
            }
        }

        if (sb.Length == 0)
        {
            return null;
        }
        if (sb[^1] == ' ')
        {
            return sb.ToString(0, sb.Length - 1);
        }
        return sb.ToString();
    }
}
