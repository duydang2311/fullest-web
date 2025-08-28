namespace WebApp.Api.Common.Http;

public sealed record ProblemError
{
    public required string Field { get; init; }
    public required string Code { get; init; }
}
