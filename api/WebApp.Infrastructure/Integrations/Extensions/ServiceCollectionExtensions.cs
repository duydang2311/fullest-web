using WebApp.Infrastructure.Integrations;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIntegrationGroup(this IServiceCollection services)
    {
        services
            .AddOptions<IntegrationKeysOptions>()
            .BindConfiguration(IntegrationKeysOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}
