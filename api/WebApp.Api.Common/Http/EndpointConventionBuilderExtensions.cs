using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Builder;

public static class EndpointConventionBuilderExtensions
{
    public static IEndpointConventionBuilder Validate<T>(this IEndpointConventionBuilder builder)
    {
        return builder.WithValidationFilter(options =>
        {
            options.InvalidResultFactory = InvalidResultFactory;
            options.ShouldValidate = static (pi, _) => pi.ParameterType == typeof(T);
        });
    }

    private static BadRequest<ProblemDetails> InvalidResultFactory(
        ValidationResult validationResult
    )
    {
        return TypedResults.BadRequest(validationResult.ToProblemDetails());
    }
}
