using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;

namespace WebApp.Application.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<UserAuth> UserAuths { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Namespace> Namespaces { get; }
    DbSet<Tag> Tags { get; }
    DbSet<TaskEntity> Tasks { get; }
    DbSet<TaskEntityAssignee> TaskAssignees { get; }
}
