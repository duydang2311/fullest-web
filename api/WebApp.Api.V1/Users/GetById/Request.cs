using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Users.GetById;

public sealed record Request(UserId UserId, string? Fields);

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Fields).NotEmpty().WithErrorCode(ErrorCodes.Required);
    }
}
