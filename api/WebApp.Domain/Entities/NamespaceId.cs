namespace WebApp.Domain.Entities;

public readonly record struct NamespaceId(long Value) : IEntityId<long> { }
