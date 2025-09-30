namespace WebApp.Domain.Entities;

public sealed record Role
{
    public RoleId Id { get; init; }
    public string Name { get; init; } = null!;
    public int Rank { get; init; }
    public ICollection<RolePermission> RolePermissions { get; init; } = null!;
}
