using System.Text.Json;
using FluentValidation;
using WebApp.Api.Common.Http;

namespace WebApp.Api.V1.Internal.Notify;

public sealed record Request
{
    public string? Kind { get; init; }
    public JsonDocument? Data { get; init; }
}

public sealed record PutUserPfpNotificationData(string UserId, string Key, long Version);

public sealed class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(a => a.Kind)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .Must(kind => string.Equals(kind, "PUT_USER_PFP", StringComparison.Ordinal))
            .WithErrorCode(ErrorCodes.Invalid);
    }
}
