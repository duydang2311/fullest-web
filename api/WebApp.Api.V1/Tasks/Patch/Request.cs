using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using NodaTime;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Patching;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.Patch;

public sealed record Request()
{
    public TaskId TaskId { get; init; }
    public TaskPatch? Patch { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }

    public sealed record TaskPatch : Patchable<TaskPatch>
    {
        public string? Title { get; init; }
        public StatusId? StatusId { get; init; }
        public PriorityId? PriorityId { get; init; }
        public Instant? DueTime { get; init; }
        public string? DueTz { get; init; }
    }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Patch).NotNull().WithErrorCode(ErrorCodes.Required);
        When(
            a => a.Patch is not null,
            () =>
            {
                RuleFor(a => a.Patch)
                    .Must(a =>
                        a!.PresentProperties.Any(p =>
                            p
                                is nameof(Request.TaskPatch.Title)
                                    or nameof(Request.TaskPatch.StatusId)
                                    or nameof(Request.TaskPatch.PriorityId)
                                    or nameof(Request.TaskPatch.DueTime)
                                    or nameof(Request.TaskPatch.DueTz)
                        )
                    )
                    .WithErrorCode(ErrorCodes.Invalid)
                    .WithMessage("At least one property must be updated.");
                RuleFor(a => a.Patch!.Title)
                    .NotEmpty()
                    .When(a => a.Patch!.Has(p => p.Title))
                    .WithErrorCode(ErrorCodes.Required);
            }
        );
    }
}
