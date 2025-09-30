using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Infrastructure.AccessControl;

public sealed class Authorizer(HybridCache cache, AppDbContext db) : IAuthorizer
{
    private const string ProjectPermissionsKey = "user:{0}:project:{1}:permissions";

    public async ValueTask<bool> HasProjectPermissionAsync(
        UserId userId,
        ProjectId projectId,
        string permission,
        CancellationToken ct = default
    )
    {
        var set = await GetOrCreatePermissionsAsync(userId, projectId, ct).ConfigureAwait(false);
        return set?.Contains(permission) ?? false;
    }

    public async ValueTask<bool> HasAllProjectPermissionsAsync(
        UserId userId,
        ProjectId projectId,
        IEnumerable<string> permissions,
        CancellationToken ct = default
    )
    {
        var set = await GetOrCreatePermissionsAsync(userId, projectId, ct).ConfigureAwait(false);
        return set?.IsSupersetOf(permissions) ?? false;
    }

    private ValueTask<HashSet<string>> GetOrCreatePermissionsAsync(
        UserId userId,
        ProjectId projectId,
        CancellationToken ct
    )
    {
        return cache.GetOrCreateAsync(
            string.Format(ProjectPermissionsKey, userId.Value, projectId.Value),
            (db, userId, projectId),
            static async (state, ct) =>
            {
                var (db, userId, projectId) = state;
                return await db
                    .ProjectMembers.Where(a => a.UserId == userId && a.ProjectId == projectId)
                    .SelectMany(a => a.Role.RolePermissions.Select(b => b.Permission.Name))
                    .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, ct)
                    .ConfigureAwait(false);
            },
            cancellationToken: ct
        );
    }
}
