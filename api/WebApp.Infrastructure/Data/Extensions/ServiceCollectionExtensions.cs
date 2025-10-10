using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Application.Data;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataGroup(this IServiceCollection services)
    {
        services
            .AddOptions<DataOptions>()
            .BindConfiguration(DataOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddDbContextPool<AppDbContext>(
            (provider, builder) =>
            {
                var dataOptions = provider.GetRequiredService<IOptions<DataOptions>>().Value;
                Configure(builder, dataOptions);
            }
        );
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<BaseDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<SeedDatabase>();
        return services;
    }

    public static void Configure(DbContextOptionsBuilder builder, DataOptions dataOptions)
    {
        builder
            .UseNpgsql(
                dataOptions.ConnectionString,
                options =>
                    options
                        .UseNodaTime()
                        .MigrationsAssembly(dataOptions.MigrationsAssembly)
                        .MapEnum<NamespaceKind>("namespace_kind")
                        .MapEnum<StatusCategory>("status_category")
            )
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .EnableServiceProviderCaching()
            .EnableThreadSafetyChecks()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention()
            .UseExceptionProcessor();
    }
}
