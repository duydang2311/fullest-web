using System.Text.Json;
using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Api.Common.Codecs;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Internal.Notify;

public sealed class Endpoint(
    AppDbContext db,
    IOptions<JsonOptions> jsonOptions,
    INumberEncoder numberEncoder
) : Endpoint<Request, Results<BadRequest, Ok>>
{
    public override void Configure()
    {
        Post("internal/notifications");
        Version(1);
        AuthSchemes("Service");
    }

    public override async Task<Results<BadRequest, Ok>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        Guard.Against.Null(req.Kind);
        switch (req.Kind)
        {
            case "PUT_USER_PFP":
                var data = req.Data?.RootElement.Deserialize<PutUserPfpNotificationData>(
                    jsonOptions.Value.SerializerOptions
                );
                if (data is null || !numberEncoder.TryDecode(data.UserId, out long userIdLong))
                {
                    return TypedResults.BadRequest();
                }
                await db
                    .Database.ExecuteSqlAsync(
                        $"""
                            update users set
                                image_key = {data.Key},
                                image_version = {data.Version}
                            where id = {userIdLong}
                        """,
                        ct
                    )
                    .ConfigureAwait(false);
                return TypedResults.Ok();
            default:
                throw new InvalidOperationException();
        }
    }
}
