using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.Create;

public sealed record Request(ProjectId ProjectId, string? Title, string? Description)
{
    public string? NormalizedTitle => Title?.Trim();

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.NormalizedTitle)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .WithName(nameof(Request.Title));
    }
}
