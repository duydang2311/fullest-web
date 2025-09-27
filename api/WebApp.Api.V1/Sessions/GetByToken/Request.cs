using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;

namespace WebApp.Api.V1.Sessions.GetByToken;

public sealed record Request(string Token, string? Fields) : IProjectableRequest;

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Token).NotEmpty().WithErrorCode(ErrorCodes.Required);
    }
}
