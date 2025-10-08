using WebApp.Domain.Entities;

namespace WebApp.Domain.Constants;

public sealed class ProjectRoleDefaults(RoleId id, string name, int rank, string[] permissions)
{
    public RoleId Id { get; } = id;
    public string Name { get; } = name;
    public int Rank { get; } = rank;
    public string[] Permissions { get; } = permissions;

    public static readonly ProjectRoleDefaults Viewer = new(
        id: new RoleId(1),
        name: "Viewer",
        rank: 100,
        permissions: [Permit.ReadTask]
    );
    public static readonly ProjectRoleDefaults Contributor = new(
        id: new RoleId(2),
        name: "Contributor",
        rank: 200,
        permissions: [.. Viewer.Permissions, Permit.ReadTask, Permit.CreateTask, Permit.UpdateTask]
    );
    public static readonly ProjectRoleDefaults Member = new(
        id: new RoleId(3),
        name: "Member",
        rank: 300,
        permissions: [.. Contributor.Permissions]
    );
    public static readonly ProjectRoleDefaults Manager = new(
        id: new RoleId(4),
        name: "Manager",
        rank: 400,
        permissions: [.. Member.Permissions, Permit.DeleteTask]
    );
    public static readonly ProjectRoleDefaults Owner = new(
        id: new RoleId(5),
        name: "Owner",
        rank: 500,
        permissions: [.. Manager.Permissions]
    );

    public static readonly ProjectRoleDefaults[] AllRoles =
    [
        Viewer,
        Contributor,
        Member,
        Manager,
        Owner,
    ];
}
