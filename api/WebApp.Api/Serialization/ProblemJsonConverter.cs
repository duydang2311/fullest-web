using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using WebApp.Api.Common.Http;

namespace WebApp.Api.Serialization;

public sealed class ProblemJsonConverter(IHttpContextAccessor httpContextAccessor)
    : JsonConverter<Problem>
{
    // source: FastEndpoints
    private static readonly Dictionary<int, string> typeMap = new()
    {
        { 400, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1" },
        { 401, "https://www.rfc-editor.org/rfc/rfc7235#section-3.1" },
        { 402, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.2" },
        { 403, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.3" },
        { 404, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.4" },
        { 405, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.5" },
        { 406, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.6" },
        { 407, "https://www.rfc-editor.org/rfc/rfc7235#section-3.2" },
        { 408, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.7" },
        { 409, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.8" },
        { 410, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.9" },
        { 411, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.10" },
        { 412, "https://www.rfc-editor.org/rfc/rfc7232#section-4.2" },
        { 413, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.11" },
        { 414, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.12" },
        { 415, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.13" },
        { 416, "https://www.rfc-editor.org/rfc/rfc7233#section-4.4" },
        { 417, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.14" },
        { 418, "https://datatracker.ietf.org/doc/html/rfc2324#section-2.3.2" },
        { 421, "https://www.rfc-editor.org/rfc/rfc7540#section-9.1.2" },
        { 422, "https://www.rfc-editor.org/rfc/rfc4918#section-11.2" },
        { 423, "https://www.rfc-editor.org/rfc/rfc4918#section-11.3" },
        { 424, "https://www.rfc-editor.org/rfc/rfc4918#section-11.4" },
        { 425, "https://www.rfc-editor.org/rfc/rfc8470#section-5.2" },
        { 426, "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.15" },
        { 428, "https://www.rfc-editor.org/rfc/rfc6585#section-3" },
        { 429, "https://www.rfc-editor.org/rfc/rfc6585#section-4" },
        { 431, "https://www.rfc-editor.org/rfc/rfc6585#section-5" },
        { 451, "https://www.rfc-editor.org/rfc/rfc7725#section-3" },
    };

    public override Problem? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, Problem value, JsonSerializerOptions options)
    {
        var httpContext =
            value.Status == null || value.TraceId == null || value.Instance == null
                ? httpContextAccessor.HttpContext
                : null;
        var status = value.Status ?? httpContext?.Response.StatusCode ?? 400;
        var traceId = value.TraceId ?? httpContext?.TraceIdentifier;
        var instance =
            value.Instance
            ?? (
                httpContext is not null
                    ? $"{httpContext.Request.Method} {httpContext.Request.Path}"
                    : null
            );
        writer.WriteStartObject();
        if (typeMap.TryGetValue(status, out var type))
        {
            writer.WriteString("type", type);
        }
        writer.WriteString("title", ReasonPhrases.GetReasonPhrase(status));
        writer.WriteNumber("status", status);
        if (value.Detail is not null)
        {
            writer.WriteString("detail", value.Detail);
        }
        if (instance is not null)
        {
            writer.WriteString("instance", instance);
        }
        if (traceId is not null)
        {
            writer.WriteString("traceId", traceId);
        }
        if (value.Errors is not null && value.Errors.Count > 0)
        {
            writer.WritePropertyName("errors");
            writer.WriteStartArray();
            foreach (var error in value.Errors)
            {
                writer.WriteStartObject();
                writer.WriteString("field", error.Field);
                writer.WriteString("code", error.Code);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }
}
