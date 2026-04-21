using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.TaskAssignees.Batch;

public sealed record Request
{
    public TaskId? TaskId { get; init; }
    public UserId[]? Assigned { get; init; }
    public UserId[]? Unassigned { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.TaskId).NotNull().WithErrorCode(ErrorCodes.Required);
        When(
            a => a.Assigned is null,
            () =>
            {
                RuleFor(x => x.Unassigned).NotEmpty().WithErrorCode(ErrorCodes.Required);
            }
        );
        When(
            a => a.Unassigned is null,
            () =>
            {
                RuleFor(x => x.Assigned).NotEmpty().WithErrorCode(ErrorCodes.Required);
            }
        );
    }
}
