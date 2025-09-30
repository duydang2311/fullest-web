namespace WebApp.Domain.Entities;

public readonly record struct PermissionId(long Value) : IEntityId<long> { }
