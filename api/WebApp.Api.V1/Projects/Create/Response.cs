using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.Create;

public sealed record Response
{
    public ProjectId ProjectId { get; init; }
}
