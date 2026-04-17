using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.Create;

public sealed record Request(ProjectId? ProjectId, string? Title, string? DescriptionJson)
{
    public string? NormalizedTitle => Title?.Trim();

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.ProjectId).NotNull().WithErrorCode(ErrorCodes.Required);
        RuleFor(x => x.NormalizedTitle)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .WithName(nameof(Request.Title));
    }
}
