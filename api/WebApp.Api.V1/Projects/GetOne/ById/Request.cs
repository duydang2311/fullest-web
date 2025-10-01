using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.GetOne.ById;

public sealed record Request(ProjectId ProjectId, string? Fields);
