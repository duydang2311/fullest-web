namespace WebApp.Domain.Common;

public sealed class Ok<A>(A value)
{
    public A Value => value;
}

public static class Ok
{
    public static Ok<A> From<A>(A value) => new(value);
}
