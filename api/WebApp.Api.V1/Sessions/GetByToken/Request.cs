using FluentValidation;
using WebApp.Api.Common.Http;

namespace WebApp.Api.V1.Sessions.GetByToken;

public sealed record Request(string Token, string? Fields);

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Token).NotEmpty().WithErrorCode(ErrorCodes.Required);
    }
}
