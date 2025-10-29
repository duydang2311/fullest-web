using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApp.Api.Common.Codecs;
using WebApp.Infrastructure.Storages;

namespace WebApp.Api.V1.Users.CreateProfileUploadRequest;

public sealed class Endpoint(
    IOptions<AssetStorageOptions> assetStorageOptions,
    INumberEncoder numberEncoder
) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Post("users/profile-upload-request");
        Version(1);
        Description(a => a.ClearDefaultAccepts().Accepts<Request>(true, "*/*"));
    }

    public override Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(assetStorageOptions.Value.SigningPrivateKeyPem);
        var signingKey = new RsaSecurityKey(rsa);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);
        var userIdEncoded = numberEncoder.Encode(req.CallerId.Value);
        var key = $"/users/{userIdEncoded}/pfp";
        var utcNow = DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "api",
            Audience = "workers-assets",
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString("D"),
                [JwtRegisteredClaimNames.Sub] = userIdEncoded,
                ["object_key"] = key,
                ["mime_type"] = "image/*",
            },
            IssuedAt = utcNow,
            NotBefore = utcNow,
            Expires = utcNow.AddMinutes(1),
            SigningCredentials = signingCredentials,
        };
        var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };
        return Task.FromResult(TypedResults.Ok(new Response(handler.CreateToken(descriptor), key)));
    }
}
