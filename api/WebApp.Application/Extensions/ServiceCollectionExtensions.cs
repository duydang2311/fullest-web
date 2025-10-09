using WebApp.Application.Features.ProjectMembers.Create;
using WebApp.Application.Features.ProjectStatuses.Create;
using WebApp.Domain.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventHandlersGroup(this IServiceCollection services)
    {
        services.AddScoped<IProjectCreatedHandler, AddProjectCreatorAsProjectOwner>();
        services.AddScoped<IProjectCreatedHandler, CreateDefaultProjectStatuses>();
        return services;
    }
}
