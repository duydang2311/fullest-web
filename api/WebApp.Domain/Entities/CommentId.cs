namespace WebApp.Domain.Entities;

public readonly record struct CommentId(long Value) : IEntityId<long>;
