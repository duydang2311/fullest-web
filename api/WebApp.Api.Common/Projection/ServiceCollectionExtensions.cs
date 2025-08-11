using System.Text.Json;
using WebApp.Api.Common.Projection;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectionGroup(this IServiceCollection services)
    {
        services.AddSingleton<IProjectionService, ProjectionService>();
        return services;
    }
}
