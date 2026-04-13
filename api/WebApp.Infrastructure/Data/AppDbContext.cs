using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApp.Application.Data;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data.Converters;

namespace WebApp.Infrastructure.Data;

public sealed class AppDbContext
    : BaseDbContext,
        IAppDbContext,
        IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext()
        : base() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public AppDbContext CreateDbContext(string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();
        var dataOptions =
            configuration.GetSection(DataOptions.Section).Get<DataOptions>()
            ?? throw new InvalidOperationException("Expected DataOptions to be configured.");
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
#pragma warning disable CS0436 // Type conflicts with imported type
        ServiceCollectionExtensions.Configure(optionsBuilder, dataOptions);
#pragma warning restore CS0436 // Type conflicts with imported type

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
        configurationBuilder
            .Properties<PriorityId>()
            .HaveConversion<EntityIdConverter<PriorityId, long>>();
        configurationBuilder
            .Properties<CommentId>()
            .HaveConversion<EntityIdConverter<CommentId, long>>();
        configurationBuilder
            .Properties<ActivityId>()
            .HaveConversion<EntityIdConverter<ActivityId, long>>();
        configurationBuilder
            .Properties<EntityId>()
            .HaveConversion<EntityIdConverter<EntityId, long>>();
        configurationBuilder
            .Properties<ActivityKind>()
            .HaveConversion<EnumToNumberConverter<ActivityKind, int>>();
        configurationBuilder
            .Properties<ActivityResourceKind>()
            .HaveConversion<EnumToNumberConverter<ActivityResourceKind, byte>>();
        configurationBuilder
            .Properties<ActivityContextKind>()
            .HaveConversion<EnumToNumberConverter<ActivityContextKind, byte>>();
    }
}
