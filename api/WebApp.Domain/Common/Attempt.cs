using System.Diagnostics.CodeAnalysis;

namespace WebApp.Domain.Common;

public sealed class Attempt<A, E>
{
    private readonly bool isOk;
    private readonly A? value;
    private readonly E? error;

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk => isOk;

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailed => !isOk;

    public A? Value => value;
    public E? Error => error;

    private Attempt(bool isOk, A? value, E? error)
    {
        this.isOk = isOk;
        this.value = value;
        this.error = error;
    }

    public static Attempt<A, E> Ok(A value) => new(true, value, default);

    public static Attempt<A, E> Failed(E error) => new(false, default, error);

    public bool TryGetValue([NotNullWhen(true)] out A? value)
    {
        if (isOk)
        {
            value = this.value!;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetError([NotNullWhen(true)] out E? value)
    {
        if (!isOk)
        {
            value = error!;
            return true;
        }

        value = default;
        return false;
    }

    public static implicit operator Attempt<A, E>(Ok<A> ok) => new(true, ok.Value, default);

    public static implicit operator Attempt<A, E>(A value) => new(true, value, default);

    public static implicit operator Attempt<A, E>(Failed<E> failed) =>
        new(false, default, failed.Error);

    public static implicit operator Attempt<A, E>(E error) => new(false, default, error);
}

public static class Attempt
{
    public static Ok<A> Ok<A>(A value) => new(value);

    public static Failed<E> Failed<E>(E error) => new(error);
}
