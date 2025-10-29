using System.Security.Cryptography;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApp.Api.Common.Codecs;
using WebApp.Infrastructure.Jwts;
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
        var userIdEncoded = numberEncoder.Encode(req.CallerId.Value);
        var key = $"/users/{userIdEncoded}/pfp";
        var utcNow = DateTime.UtcNow;
        return Task.FromResult(
            TypedResults.Ok(
                new Response(
                    JwtHelper.CreateToken(
                        assetStorageOptions.Value.SigningPrivateKeyPem,
                        iss: "api",
                        aud: "workers-assets",
                        claims: new()
                        {
                            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString("D"),
                            [JwtRegisteredClaimNames.Sub] = userIdEncoded,
                            ["object_key"] = key,
                            ["mime_type"] = "image/*",
                        },
                        iat: utcNow,
                        nbf: utcNow,
                        exp: utcNow.AddMinutes(1)
                    ),
                    key
                )
            )
        );
    }
}
