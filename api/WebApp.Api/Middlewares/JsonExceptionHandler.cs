using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Common.Http;

namespace WebApp.Api.Middlewares;

public class JsonExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (
            exception.InnerException is not JsonException jsonException
            || jsonException.Path is null
        )
        {
            return false;
        }

        ValidationProblemDetails problemDetails;
        if (jsonException.Path.Equals("$", StringComparison.Ordinal))
        {
            problemDetails = new ValidationProblemDetails
            {
                Extensions = { { "codes", ErrorCodes.Json } },
            };
        }
        else
        {
            var dotIndex = jsonException.Path.IndexOf('.', StringComparison.Ordinal);
            var path = dotIndex == -1 ? jsonException.Path : jsonException.Path[(dotIndex + 1)..];
            problemDetails = new ValidationProblemDetails(
                new Dictionary<string, string[]>() { { path, new[] { ErrorCodes.Json } } }
            );
        }

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception,
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await problemDetailsService.TryWriteAsync(context);
    }
}
