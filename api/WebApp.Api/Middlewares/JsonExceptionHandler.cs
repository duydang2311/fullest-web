using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using WebApp.Api.Common.Http;

namespace WebApp.Api.Middlewares;

public class JsonExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        JsonException? jsonException = exception switch
        {
            JsonException e => e,
            { InnerException: JsonException inner } => inner,
            _ => null,
        };
        if (jsonException?.Path is null)
        {
            return false;
        }

        var dotIndex = jsonException.Path.IndexOf('.', StringComparison.Ordinal);
        var path = dotIndex == -1 ? jsonException.Path : jsonException.Path[(dotIndex + 1)..];

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(
            Problem.FromError(path, ErrorCodes.Json),
            cancellationToken: cancellationToken
        );
        return true;
    }
}
