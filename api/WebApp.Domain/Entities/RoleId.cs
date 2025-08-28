namespace WebApp.Domain.Entities;

public readonly record struct RoleId(long Value) : IEntityId<long> { }
