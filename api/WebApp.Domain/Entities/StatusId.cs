namespace WebApp.Domain.Entities;

public readonly record struct StatusId(long Value) : IEntityId<long> { }
