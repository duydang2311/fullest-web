using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.GetMany;

public sealed record Request
{
    public UserId? MemberId { get; init; }
    public string? Select { get; init; }

    public int Size { get; init; } = 20;
    public Direction Direction { get; init; }
    public ProjectId? AfterId { get; init; }
    public ProjectId? UntilId { get; init; }

    [FromClaim(ClaimTypes.NameIdentifier)]
    public UserId CallerId { get; init; }
}

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.MemberId).NotNull().WithErrorCode(ErrorCodes.Required);
    }
}
