namespace WebApp.Domain.Entities;

public readonly record struct UserId(long Value) : IEntityId<long>
{
    public static bool operator >(UserId a, UserId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(UserId a, UserId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator >=(UserId a, UserId b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(UserId a, UserId b)
    {
        return a.Value <= b.Value;
    }
}
