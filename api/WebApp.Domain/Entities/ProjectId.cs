namespace WebApp.Domain.Entities;

public readonly record struct ProjectId(long Value) : IEntityId<long>
{
    public static bool operator >(ProjectId a, ProjectId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(ProjectId a, ProjectId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator >=(ProjectId a, ProjectId b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(ProjectId a, ProjectId b)
    {
        return a.Value <= b.Value;
    }
}
