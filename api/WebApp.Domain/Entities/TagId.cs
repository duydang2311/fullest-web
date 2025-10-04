namespace WebApp.Domain.Entities;

public readonly record struct TagId(long Value) : IEntityId<long> { }
