namespace WebApp.Domain.Entities;

public readonly record struct LabelId(long Value) : IEntityId<long> { }
