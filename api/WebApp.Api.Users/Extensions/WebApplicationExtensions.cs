using WebApp.Api.Users.V1;

namespace Microsoft.AspNetCore.Builder;

public static partial class WebApplicationExtensions
{
    public static void MapUserApiV1(this WebApplication app)
    {
        var v1 = app.NewVersionedApi("Users")
            .MapGroup("/api/v{version:apiVersion}/users")
            .HasApiVersion(1);
        v1.MapGet("/{userId}", GetUserById.HandleAsync);
        v1.MapPost("/", CreateUser.HandleAsync).AllowAnonymous().Validate<CreateUser.Request>();
    }
}
