namespace WebApp.Domain.Entities;

public readonly record struct CommentId(long Value) : IEntityId<long>
{
    public static bool operator >(CommentId a, CommentId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(CommentId a, CommentId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator >=(CommentId a, CommentId b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(CommentId a, CommentId b)
    {
        return a.Value <= b.Value;
    }
}
