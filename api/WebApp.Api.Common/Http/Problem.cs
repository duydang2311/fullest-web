using System.Collections.Immutable;
using FastEndpoints;

namespace WebApp.Api.Common.Http;

public sealed record Problem : IResult
{
    private ImmutableList<ProblemError>? errors;

    public int? Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }
    public string? TraceId { get; init; }
    public IReadOnlyCollection<ProblemError>? Errors => errors;

    public Problem() { }

    public Problem(IEnumerable<ProblemError> errors)
    {
        this.errors = [.. errors];
    }

    public static Problem FromStatus(int status)
    {
        return new Problem { Status = status };
    }

    public static Problem FromDetail(string detail)
    {
        return new Problem { Detail = detail };
    }

    public static Problem FromError(string field, string code)
    {
        return new Problem { errors = [new() { Field = field, Code = code }] };
    }

    public static Problem FromError(string code)
    {
        return FromError("$", code);
    }

    public Problem Error(string field, string error)
    {
        return this with
        {
            errors = errors is null
                ? [new ProblemError() { Field = field, Code = error }]
                : errors.Add(new ProblemError() { Field = field, Code = error }),
        };
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        return httpContext.Response.SendAsync(
            this with
            {
                TraceId = TraceId ?? httpContext.TraceIdentifier,
                Instance = Instance ?? $"{httpContext.Request.Method} {httpContext.Request.Path}",
            },
            Status ?? httpContext.Response.StatusCode
        );
    }
}
