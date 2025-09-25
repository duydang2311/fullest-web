namespace WebApp.Domain.Common;

public sealed class Failed<E>(E error)
{
    public E Error => error;
}

public static class Failed
{
    public static Failed<E> From<E>(E error) => new(error);
}
