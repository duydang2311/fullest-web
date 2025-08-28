namespace WebApp.Domain.Entities;

public sealed record RolePermission
{
    public long Id { get; init; }
    public RoleId RoleId { get; init; }
    public Role Role { get; init; } = null!;
    public string Permission { get; init; } = null!;
}
