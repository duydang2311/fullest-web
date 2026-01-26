namespace WebApp.Domain.Entities;

public readonly record struct ActivityId(long Value) : IEntityId<long>
{
    public static bool operator >(ActivityId a, ActivityId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(ActivityId a, ActivityId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator >=(ActivityId a, ActivityId b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(ActivityId a, ActivityId b)
    {
        return a.Value <= b.Value;
    }
}
