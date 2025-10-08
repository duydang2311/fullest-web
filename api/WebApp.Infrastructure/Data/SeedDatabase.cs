using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data;

public sealed class SeedDatabase(AppDbContext db)
{
    public async Task EnsureRolesAsync(CancellationToken ct = default)
    {
        var permissionDict = await EnsurePermissionsInternalAsync(ct).ConfigureAwait(false);
        await EnsureRolesInternalAsync(permissionDict, ct).ConfigureAwait(false);
        await EnsureRolePermissionsInternalAsync(permissionDict, ct).ConfigureAwait(false);
    }

    private async Task<Dictionary<string, Permission>> EnsurePermissionsInternalAsync(
        CancellationToken ct = default
    )
    {
        var permissions = typeof(Permit)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(a => a.FieldType == typeof(string))
            .Select(a => (string)a.GetValue(null)!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingPermissions = await db.Permissions.ToListAsync(ct).ConfigureAwait(false);
        var existingPermissionNames = existingPermissions
            .Select(a => a.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var adding = permissions
            .Except(existingPermissions.Select(a => a.Name), StringComparer.OrdinalIgnoreCase)
            .ToList();
        var removing = existingPermissionNames
            .Except(permissions, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var addingPermissions = adding.Select(a => new Permission { Name = a }).ToList();
        existingPermissions.AddRange(addingPermissions);
        await db.AddRangeAsync(addingPermissions, ct).ConfigureAwait(false);
        if (removing.Count > 0)
        {
            await db
                .Permissions.Where(a => removing.Contains(a.Name))
                .ExecuteDeleteAsync(ct)
                .ConfigureAwait(false);
        }
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        return existingPermissions.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
    }

    private async Task EnsureRolesInternalAsync(
        Dictionary<string, Permission> permissionDict,
        CancellationToken ct = default
    )
    {
        var allRoleIds = ProjectRoleDefaults.AllRoles.Select(a => a.Id);
        var existingRoles = await db.Roles.ToListAsync(ct).ConfigureAwait(false);
        var existingRoleIds = existingRoles.Select(a => a.Id).ToList();
        var addingRoleIds = allRoleIds.Except(existingRoleIds).ToList();
        var removingRoleIds = existingRoleIds.Except(allRoleIds).ToList();
        var addingRoles = addingRoleIds.Select(a =>
            ProjectRoleDefaults.AllRoles.First(b => b.Id == a)
        );
        var removingRoles = removingRoleIds
            .Select(a => ProjectRoleDefaults.AllRoles.First(b => b.Id == a))
            .ToList();

        await db.AddRangeAsync(
                addingRoles.Select(a => new Role
                {
                    Id = a.Id,
                    Name = a.Name,
                    Rank = a.Rank,
                    Permissions = [.. a.Permissions.Select(b => permissionDict[b])],
                }),
                ct
            )
            .ConfigureAwait(false);
        if (removingRoles.Count > 0)
        {
            await db
                .Roles.Where(a => removingRoleIds.Contains(a.Id))
                .ExecuteDeleteAsync(ct)
                .ConfigureAwait(false);
        }
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task EnsureRolePermissionsInternalAsync(
        Dictionary<string, Permission> permissionDict,
        CancellationToken ct = default
    )
    {
        var existingRoles = await db
            .Roles.Include(a => a.Permissions)
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
        foreach (var role in ProjectRoleDefaults.AllRoles)
        {
            var permissionNames = role.Permissions;
            var existingPermissions = existingRoles[role.Id].Permissions;
            var existingPermissionNames = existingPermissions
                .Select(a => a.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var adding = permissionNames
                .Except(existingPermissions.Select(a => a.Name), StringComparer.OrdinalIgnoreCase)
                .ToList();
            var removing = existingPermissionNames
                .Except(permissionNames, StringComparer.OrdinalIgnoreCase)
                .Select(a => permissionDict[a].Id)
                .ToList();
            var addingPermissions = adding
                .Select(a => new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionDict[a].Id,
                })
                .ToList();
            await db.AddRangeAsync(addingPermissions, ct).ConfigureAwait(false);
            if (removing.Count > 0)
            {
                await db
                    .RolePermissions.Where(a =>
                        a.RoleId == role.Id && removing.Contains(a.PermissionId)
                    )
                    .ExecuteDeleteAsync(ct)
                    .ConfigureAwait(false);
            }
        }
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
