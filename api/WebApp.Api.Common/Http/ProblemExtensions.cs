using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApp.Api.Common.Http;

public static class ProblemExtensions
{
    public static Problem Status(this Problem problem, int status)
    {
        return problem with { Status = status };
    }

    public static Problem Detail(this Problem problem, string detail)
    {
        return problem with { Detail = detail };
    }

    public static Problem Instance(this Problem problem, string instance)
    {
        return problem with { Instance = instance };
    }

    public static Problem TraceId(this Problem problem, string traceId)
    {
        return problem with { TraceId = traceId };
    }

    public static Problem Error(this Problem problem, string error)
    {
        return problem.Error("$", error);
    }

    public static ProblemHttpResult BuildHttpResult(this Problem problem)
    {
        return TypedResults.Problem(
            detail: problem.Detail,
            instance: problem.Instance,
            statusCode: problem.Status,
            extensions: [KeyValuePair.Create<string, object?>("errors", problem.Errors)]
        );
    }
}
