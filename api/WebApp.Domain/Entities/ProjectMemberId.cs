namespace WebApp.Domain.Entities;

public readonly record struct ProjectMemberId(long Value) : IEntityId<long> { }
