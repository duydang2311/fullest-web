using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data.Converters;

namespace WebApp.Infrastructure.Data;

public sealed class AppDbContext : DbContext, IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext()
        : base() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();

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
            .Properties<AuthProvider>()
            .HaveConversion<EnumToStringConverter<AuthProvider>>();
    }
}
