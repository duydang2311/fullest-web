using System.Text.Json;
using WebApp.Api.Common.Http;

namespace FluentValidation.Results;

public static class FluentValidationResultExtensions
{
    public static Problem ToProblem(
        this ValidationResult validationResult,
        Func<string, string>? convertPropertyName = null
    )
    {
        return validationResult.Errors.ToProblem(convertPropertyName);
    }

    public static Problem ToProblem(
        this IEnumerable<ValidationFailure> failures,
        Func<string, string>? convertPropertyName = null
    )
    {
        var errors = failures
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray(),
                StringComparer.Ordinal
            );

        var problem = new Problem();
        foreach (var error in failures)
        {
            problem.Error(
                (convertPropertyName ?? JsonNamingPolicy.CamelCase.ConvertName)(error.PropertyName),
                error.ErrorCode ?? error.ErrorMessage
            );
        }
        return problem;
    }
}
