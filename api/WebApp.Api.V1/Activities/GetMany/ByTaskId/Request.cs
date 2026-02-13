using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId;

public sealed record Request
{
    public TaskId? TaskId { get; init; }
    public string? Fields { get; init; }
    public ActivityId? After { get; init; }
    public int Size { get; init; } = 20;

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.TaskId).NotNull().WithErrorCode(ErrorCodes.Required);
    }
}
