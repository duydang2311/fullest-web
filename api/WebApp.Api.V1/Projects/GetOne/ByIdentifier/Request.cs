using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Projects.GetOne.ByIdentifier;

public sealed record Request(NamespaceId NamespaceId, string Identifier, string? Fields);
