using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Infrastructure.Jwts;

public static class JwtHelper
{
    public static string CreateToken(
        string privateKeyPem,
        string? iss = null,
        string? aud = null,
        DateTime? iat = null,
        DateTime? nbf = null,
        DateTime? exp = null,
        Dictionary<string, object>? claims = null
    )
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };
        return handler.CreateToken(
            new SecurityTokenDescriptor
            {
                Issuer = iss,
                Audience = aud,
                IssuedAt = iat,
                NotBefore = nbf,
                Expires = exp,
                Claims = claims,
                SigningCredentials = new SigningCredentials(
                    new RsaSecurityKey(rsa),
                    SecurityAlgorithms.RsaSha256
                ),
            }
        );
    }

    public static Task<TokenValidationResult> VerifyTokenAsync(
        string publicKeyPem,
        string token,
        string? iss = null,
        string? aud = null
    )
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        var handler = new JsonWebTokenHandler();
        return handler.ValidateTokenAsync(
            token,
            new TokenValidationParameters
            {
                ValidateIssuer = iss is not null,
                ValidIssuer = iss,
                ValidateAudience = aud is not null,
                ValidAudience = aud,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
            }
        );
    }
}
