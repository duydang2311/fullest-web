using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Constants;

namespace WebApp.Api.V1.Sessions.Create;

public sealed record Request
{
    public AuthProvider? Provider { get; init; }
    public string? GoogleIdToken { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Provider)
            .NotNull()
            .WithErrorCode(ErrorCodes.Required)
            .IsInEnum()
            .WithErrorCode(ErrorCodes.Invalid);
        When(
            a => a.Provider == AuthProvider.Google,
            () =>
            {
                RuleFor(x => x.GoogleIdToken).NotEmpty().WithErrorCode(ErrorCodes.Required);
            }
        );
    }
}
