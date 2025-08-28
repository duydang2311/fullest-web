namespace WebApp.Domain.Entities;

public readonly record struct UserSessionId(long Value) : IEntityId<long> { }
