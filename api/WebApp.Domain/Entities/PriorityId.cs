namespace WebApp.Domain.Entities;

public readonly record struct PriorityId(long Value) : IEntityId<long> { }
