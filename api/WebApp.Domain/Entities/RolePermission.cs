namespace WebApp.Domain.Entities;

public sealed record RolePermission
{
    public RoleId RoleId { get; init; }
    public Role Role { get; init; } = null!;
    public PermissionId PermissionId { get; init; }
    public Permission Permission { get; init; } = null!;
}
