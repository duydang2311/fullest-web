using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Patching;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Comments.Patch;

public sealed record Request(CommentId? CommentId, Request.CommentPatch? Patch)
{
    public sealed record CommentPatch : Patchable<CommentPatch>
    {
        public string? ContentJson { get; init; }
    }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.CommentId).NotNull().WithErrorCode(ErrorCodes.Required);
        RuleFor(a => a.Patch).NotNull().WithErrorCode(ErrorCodes.Required);
        When(
            a => a.Patch is not null,
            () =>
            {
                RuleFor(a => a.Patch)
                    .Must(a => a!.ContentJson is not null)
                    .WithErrorCode(ErrorCodes.Invalid);
            }
        );
    }
}
