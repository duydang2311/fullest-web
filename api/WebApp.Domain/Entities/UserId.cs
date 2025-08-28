namespace WebApp.Domain.Entities;

public readonly record struct UserId(long Value) : IEntityId<long> { }
