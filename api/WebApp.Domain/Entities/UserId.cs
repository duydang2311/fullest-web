using System.Diagnostics.CodeAnalysis;

namespace WebApp.Domain.Entities;

public readonly record struct UserId(long Value) : IEntityId<long>
{
    public static bool TryParse([NotNullWhen(true)] string? s, out UserId userId)
    {
        if (long.TryParse(s, out var value))
        {
            userId = new UserId(value);
            return true;
        }

        userId = default;
        return false;
    }
}
