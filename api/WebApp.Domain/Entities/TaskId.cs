namespace WebApp.Domain.Entities;

public readonly record struct TaskId(long Value) : IEntityId<long> { }
