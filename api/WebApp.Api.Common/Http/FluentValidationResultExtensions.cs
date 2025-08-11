using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Common.Http;

namespace FluentValidation.Results;

public static class FluentValidationResultExtensions
{
    public static Problem ToProblem(
        this ValidationResult validationResult,
        Func<string, string>? convertPropertyName = null
    )
    {
        var errors = validationResult
            .Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

        var problem = new Problem();
        foreach (var error in validationResult.Errors)
        {
            problem.Error(
                (convertPropertyName ?? JsonNamingPolicy.CamelCase.ConvertName)(error.PropertyName),
                error.ErrorCode ?? error.ErrorMessage
            );
        }
        return problem;
    }

    public static ProblemDetails ToProblemDetails(
        this ValidationResult validationResult,
        Func<string, string>? convertPropertyName = null
    )
    {
        return validationResult.ToProblem(convertPropertyName).Build();
    }
}
