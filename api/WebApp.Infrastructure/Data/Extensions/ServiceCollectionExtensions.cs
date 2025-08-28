using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        return services;
    }

    public static void Configure(DbContextOptionsBuilder builder, DataOptions dataOptions)
    {
        builder
            .UseNpgsql(
                dataOptions.ConnectionString,
                options => options.UseNodaTime().MigrationsAssembly(dataOptions.MigrationsAssembly)
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
