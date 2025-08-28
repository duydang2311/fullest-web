using System.Buffers.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Api.Common.Codecs;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.Security;

public sealed class SessionAuthSchemeHandler(
    IOptionsMonitor<SessionAuthSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AppDbContext db,
    INumberEncoder numberEncoder
) : AuthenticationHandler<SessionAuthSchemeOptions>(options, logger, encoder)
{
    private const string SchemeName = "Session";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Context.Request.Headers.TryGetValue("Authorization", out var headerValue))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var value = headerValue.FirstOrDefault();
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var span = value.AsSpan();
        var delimiterIndex = span.IndexOf(' ');
        if (delimiterIndex == -1)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var scheme = span[..delimiterIndex];
        var token = span[(delimiterIndex + 1)..];

        if (!scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (token.IsEmpty || token.IsWhiteSpace())
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        byte[] bytes;
        try
        {
            bytes = Base64Url.DecodeFromChars(token);
        }
        catch (FormatException)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        return AuthenticateAsyncInternal(bytes, Context.RequestAborted);
    }

    private async Task<AuthenticateResult> AuthenticateAsyncInternal(
        byte[] token,
        CancellationToken ct
    )
    {
        var userId = await db
            .UserSessions.Where(a => a.Token.SequenceEqual(token))
            .Select(a => (UserId?)a.UserId)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (userId is null)
        {
            return AuthenticateResult.NoResult();
        }
        return AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [
                            new Claim(
                                ClaimTypes.NameIdentifier,
                                numberEncoder.Encode(userId.Value.Value)
                            ),
                        ],
                        SchemeName
                    )
                ),
                SchemeName
            )
        );
    }
}
