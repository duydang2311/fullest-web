namespace WebApp.Domain.Entities;

public readonly record struct ProjectId(long Value) : IEntityId<long> { }
