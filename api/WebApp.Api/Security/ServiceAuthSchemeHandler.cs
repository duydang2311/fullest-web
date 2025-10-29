using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using WebApp.Infrastructure.Integrations;
using WebApp.Infrastructure.Jwts;

namespace WebApp.Api.Security;

public sealed class ServiceAuthSchemeHandler(
    IOptionsMonitor<ServiceAuthSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<IntegrationKeysOptions> integrationKeysOptions
) : AuthenticationHandler<ServiceAuthSchemeOptions>(options, logger, encoder)
{
    private const string SchemeName = "Service";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (
            !Context.Request.Headers.TryGetValue("X-Service", out var serviceValue)
            || !Context.Request.Headers.TryGetValue("X-Service-Token", out var serviceTokenValue)
        )
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var service = serviceValue.FirstOrDefault();
        if (string.IsNullOrEmpty(service))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var serviceToken = serviceTokenValue.FirstOrDefault();
        if (string.IsNullOrEmpty(serviceToken))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        IntegrationKeysOptions.ServiceOptions? serviceOptions = default;
        if (
            service.Equals(integrationKeysOptions.Value.AssetWorkers.Name, StringComparison.Ordinal)
        )
        {
            serviceOptions = integrationKeysOptions.Value.AssetWorkers;
        }
        if (serviceOptions is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        return HandleAuthenticateInternalAsync(
            serviceOptions.PublicKeyPem,
            serviceToken,
            serviceOptions.Name,
            serviceOptions.Issuer,
            serviceOptions.Audience
        );
    }

    private static async Task<AuthenticateResult> HandleAuthenticateInternalAsync(
        string publicKeyPem,
        string token,
        string serviceName,
        string issuer,
        string audience
    )
    {
        var result = await JwtHelper
            .VerifyTokenAsync(publicKeyPem, token, iss: issuer, aud: audience)
            .ConfigureAwait(false);
        if (!result.IsValid)
        {
            return AuthenticateResult.NoResult();
        }
        return AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [new Claim(JwtRegisteredClaimNames.Sub, serviceName)],
                        SchemeName
                    )
                ),
                SchemeName
            )
        );
    }
}
