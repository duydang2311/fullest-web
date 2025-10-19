using System.Text;
using System.Text.Json;
using WebApp.Application.Data;
using WebApp.Domain.Commands;
using WebApp.Domain.Entities;

namespace WebApp.Application.Features.Statuses.Comments.Create;

public sealed class CreateCommentHandler(BaseDbContext db) : ICreateCommentHandler
{
    public async Task<Comment> HandleAsync(CreateComment command, CancellationToken ct)
    {
        var doc =
            command.ContentJson is null
            || command.ContentJson.RootElement.ValueKind == JsonValueKind.Null
                ? null
                : command.ContentJson;
        string? preview = default;
        if (doc is not null)
        {
            preview = ExtractText(doc, 256);
        }
        else if (!string.IsNullOrEmpty(command.ContentText))
        {
            doc = JsonSerializer.SerializeToDocument(
                new
                {
                    type = "doc",
                    content = new[]
                    {
                        new
                        {
                            type = "paragraph",
                            content = new[] { new { type = "text", text = command.ContentText } },
                        },
                    },
                }
            );
            preview = ExtractText(command.ContentText, 256);
        }

        var comment = new Comment
        {
            TaskId = command.TaskId,
            AuthorId = command.AuthorId,
            ContentJson = doc is null ? null : JsonSerializer.Serialize(doc),
            ContentPreview = preview,
        };
        await db.AddAsync(comment, ct).ConfigureAwait(false);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        return comment;
    }

    private static string? ExtractText(string text, int size)
    {
        if (text.Length <= size)
        {
            return text;
        }

        if (text[size] == ' ')
        {
            return text[..size];
        }

        var lastDelimiter = -1;
        for (int i = size; i >= 0; --i)
        {
            if (text[i] == ' ')
            {
                lastDelimiter = i;
                break;
            }
        }
        if (lastDelimiter == -1)
        {
            return text[..size];
        }
        return text[..lastDelimiter];
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

        if (sb[^1] == ' ')
        {
            return sb.ToString(0, sb.Length - 1);
        }
        return sb.ToString();
    }
}
