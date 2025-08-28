using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.AccessControl;

public interface IAuthorizer
{
    ValueTask<bool> HasProjectPermissionAsync(
        UserId userId,
        ProjectId projectId,
        string permission,
        CancellationToken ct = default
    );
    ValueTask<bool> HasAllProjectPermissionsAsync(
        UserId userId,
        ProjectId projectId,
        IEnumerable<string> permissions,
        CancellationToken ct = default
    );
}
