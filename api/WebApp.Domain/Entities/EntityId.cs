namespace WebApp.Domain.Entities;

public readonly record struct EntityId(long Value) : IEntityId<long>
{
    public static bool operator >(EntityId a, EntityId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(EntityId a, EntityId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator >=(EntityId a, EntityId b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(EntityId a, EntityId b)
    {
        return a.Value <= b.Value;
    }

    public static bool operator ==(EntityId a, long b)
    {
        return a.Value == b;
    }

    public static bool operator !=(EntityId a, long b)
    {
        return a.Value != b;
    }
}
