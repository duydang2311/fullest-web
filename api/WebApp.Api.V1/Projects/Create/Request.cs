using System.Security.Claims;
using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.Create;

public sealed record Request(string? Name, string? Identifier, string? Summary)
{
    public string? NormalizedIdentifier => Identifier?.Trim().ToLowerInvariant();
    public string? NormalizedSummary => Summary?.Trim();

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed partial class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .Matches(NamePattern())
            .WithErrorCode(ErrorCodes.Invalid)
            .MaximumLength(100)
            .WithErrorCode(ErrorCodes.MaxLength);
        RuleFor(a => a.NormalizedIdentifier)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .WithName(nameof(Request.Identifier))
            .Matches(IdentifierPattern())
            .WithErrorCode(ErrorCodes.Invalid)
            .WithName(nameof(Request.Identifier))
            .MaximumLength(50)
            .WithErrorCode(ErrorCodes.MaxLength)
            .WithName(nameof(Request.Identifier));
        When(
            a => !string.IsNullOrEmpty(a.NormalizedSummary),
            () =>
            {
                RuleFor(a => a.NormalizedSummary)
                    .MaximumLength(350)
                    .WithErrorCode(ErrorCodes.MaxLength)
                    .WithName(nameof(Request.Summary));
            }
        );
    }

    [GeneratedRegex("^[a-zA-Z0-9-_\\s]+$")]
    private static partial Regex NamePattern();

    [GeneratedRegex("^[a-zA-Z0-9-_]+$")]
    private static partial Regex IdentifierPattern();
}
