using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.Create;

public sealed record Response(ProjectId Id, string Identifier);
