using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApp.Application.Data;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data.Converters;

namespace WebApp.Infrastructure.Data;

public sealed class AppDbContext
    : DbContext,
        IAppDbContext,
        IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext()
        : base() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
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

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();
        var dataOptions =
            configuration.GetSection(DataOptions.Section).Get<DataOptions>()
            ?? throw new InvalidOperationException("Expected DataOptions to be configured.");
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        ServiceCollectionExtensions.Configure(optionsBuilder, dataOptions);

        return new AppDbContext(optionsBuilder.Options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<UserId>().HaveConversion<EntityIdConverter<UserId, long>>();
        configurationBuilder
            .Properties<UserAuthId>()
            .HaveConversion<EntityIdConverter<UserAuthId, long>>();
        configurationBuilder
            .Properties<UserSessionId>()
            .HaveConversion<EntityIdConverter<UserSessionId, long>>();
        configurationBuilder
            .Properties<AuthProvider>()
            .HaveConversion<EnumToStringConverter<AuthProvider>>();
        configurationBuilder.Properties<RoleId>().HaveConversion<EntityIdConverter<RoleId, long>>();
        configurationBuilder
            .Properties<ProjectId>()
            .HaveConversion<EntityIdConverter<ProjectId, long>>();
        configurationBuilder
            .Properties<ProjectMemberId>()
            .HaveConversion<EntityIdConverter<ProjectMemberId, long>>();
        configurationBuilder
            .Properties<PermissionId>()
            .HaveConversion<EntityIdConverter<PermissionId, long>>();
        configurationBuilder
            .Properties<NamespaceId>()
            .HaveConversion<EntityIdConverter<NamespaceId, long>>();
        configurationBuilder.Properties<TagId>().HaveConversion<EntityIdConverter<TagId, long>>();
        configurationBuilder.Properties<TaskId>().HaveConversion<EntityIdConverter<TaskId, long>>();
        configurationBuilder
            .Properties<LabelId>()
            .HaveConversion<EntityIdConverter<LabelId, long>>();
        configurationBuilder
            .Properties<StatusId>()
            .HaveConversion<EntityIdConverter<StatusId, long>>();
    }
}
