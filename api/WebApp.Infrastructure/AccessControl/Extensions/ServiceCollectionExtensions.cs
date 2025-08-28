using WebApp.Infrastructure.AccessControl;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAccessControlGroup(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizer, Authorizer>();
        return services;
    }
}
