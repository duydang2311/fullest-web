using System.Diagnostics.CodeAnalysis;

namespace WebApp.Domain.Entities;

public readonly record struct UserAuthId(long Value) : IEntityId<long>
{
    public static bool TryParse([NotNullWhen(true)] string? s, out UserAuthId userAuthId)
    {
        if (long.TryParse(s, out var value))
        {
            userAuthId = new UserAuthId(value);
            return true;
        }

        userAuthId = default;
        return false;
    }
}
