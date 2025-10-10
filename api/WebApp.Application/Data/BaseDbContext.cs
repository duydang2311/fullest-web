using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;

namespace WebApp.Application.Data;

public abstract class BaseDbContext : DbContext
{
    public BaseDbContext()
        : base() { }

    public BaseDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserAuth> UserAuths => Set<UserAuth>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Namespace> Namespaces => Set<Namespace>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<TaskEntityAssignee> TaskAssignees => Set<TaskEntityAssignee>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Priority> Priorities => Set<Priority>();
}
