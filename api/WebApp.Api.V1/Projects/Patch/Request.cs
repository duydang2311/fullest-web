using System.Security.Claims;
using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Patching;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.Patch;

public sealed record Request()
{
    public ProjectId ProjectId { get; init; }
    public uint Version { get; init; }
    public ProjectPatch? Patch { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }

    public sealed record ProjectPatch : Patchable<ProjectPatch>
    {
        public string? Name { get; init; }
        public string? Summary { get; init; }
        public string? DescriptionJson { get; init; }
    }
}

public sealed partial class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Patch).NotNull().WithErrorCode(ErrorCodes.Required);
        When(
            a => a.Patch is not null,
            () =>
            {
                RuleFor(a => a.Patch)
                    .Must(a => a!.PresentProperties.Count > 0)
                    .WithErrorCode(ErrorCodes.Invalid)
                    .WithMessage("At least one property must be updated.");
                When(
                    a => a.Patch is not null && a.Patch.Has(p => p.Name),
                    () =>
                    {
                        RuleFor(a => a.Patch!.Name)
                            .NotEmpty()
                            .WithErrorCode(ErrorCodes.Required)
                            .Matches(NamePattern())
                            .WithErrorCode(ErrorCodes.Invalid)
                            .MaximumLength(100)
                            .WithErrorCode(ErrorCodes.MaxLength);
                    }
                );
                RuleFor(a => a.Patch!.Summary)
                    .MaximumLength(350)
                    .When(a => a.Patch!.Has(p => p.Summary) && a.Patch!.Summary is not null)
                    .WithErrorCode(ErrorCodes.MaxLength);
            }
        );
    }

    [GeneratedRegex("^[a-zA-Z0-9-_\\s]+$")]
    private static partial Regex NamePattern();
}
