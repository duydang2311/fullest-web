using WebApp.Infrastructure.Storages;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageGroup(this IServiceCollection services)
    {
        services
            .AddOptions<AssetStorageOptions>()
            .BindConfiguration(AssetStorageOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}
