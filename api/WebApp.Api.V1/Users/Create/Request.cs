using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Constants;

namespace WebApp.Api.V1.Users.Create;

public sealed record Request(
    AuthProvider Provider,
    string Name,
    string? Password,
    string? GoogleIdToken
);

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Provider).IsInEnum().WithErrorCode(ErrorCodes.Invalid);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .MinimumLength(3)
            .WithErrorCode(ErrorCodes.MinLength)
            .MaximumLength(32)
            .WithErrorCode(ErrorCodes.MaxLength);
        When(
            a => a.Provider == AuthProvider.Credentials,
            () =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithErrorCode(ErrorCodes.Required)
                    .MinimumLength(8)
                    .WithErrorCode(ErrorCodes.MinLength)
                    .MaximumLength(64)
                    .WithErrorCode(ErrorCodes.MaxLength);
            }
        );
        When(
            a => a.Provider == AuthProvider.Google,
            () =>
            {
                RuleFor(x => x.GoogleIdToken).NotEmpty().WithErrorCode(ErrorCodes.Required);
            }
        );
    }
}
