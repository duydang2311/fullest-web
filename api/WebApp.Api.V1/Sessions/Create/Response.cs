namespace WebApp.Api.V1.Sessions.Create;

public sealed record Response
{
    public required string Token { get; init; }
}
