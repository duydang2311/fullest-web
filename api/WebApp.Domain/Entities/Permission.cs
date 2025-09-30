namespace WebApp.Domain.Entities;

public sealed record Permission
{
    public PermissionId Id { get; init; }
    public string Name { get; init; } = null!;
}
