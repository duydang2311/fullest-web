using WebApp.Application.Features.Priorities.Create;
using WebApp.Application.Features.ProjectMembers.Create;
using WebApp.Application.Features.Statuses.Comments.Create;
using WebApp.Application.Features.Statuses.Create;
using WebApp.Domain.Commands;
using WebApp.Domain.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventHandlersGroup(this IServiceCollection services)
    {
        services.AddScoped<IProjectCreatedHandler, AddProjectCreatorAsProjectOwner>();
        services.AddScoped<IProjectCreatedHandler, CreateDefaultStatuses>();
        services.AddScoped<IProjectCreatedHandler, CreateDefaultPriorities>();
        services.AddScoped<ICreateCommentHandler, CreateCommentHandler>();
        return services;
    }
}
