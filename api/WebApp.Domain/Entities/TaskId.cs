namespace WebApp.Domain.Entities;

public readonly record struct TaskId(long Value) : IEntityId<long>
{
    public static bool operator >(TaskId a, TaskId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(TaskId a, TaskId b)
    {
        return a.Value > b.Value;
    }

    public static bool operator >=(TaskId a, TaskId b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(TaskId a, TaskId b)
    {
        return a.Value <= b.Value;
    }
}
