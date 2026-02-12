using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.TaskAssignees.Create;

public sealed record Request
{
    public TaskId? TaskId { get; init; }
    public UserId? UserId { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.TaskId).NotNull().WithErrorCode(ErrorCodes.Required);
        RuleFor(x => x.UserId).NotNull().WithErrorCode(ErrorCodes.Required);
    }
}
