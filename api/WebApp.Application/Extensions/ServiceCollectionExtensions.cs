using WebApp.Application.Features.Activities.Create;
using WebApp.Application.Features.Activities.Delete;
using WebApp.Application.Features.Comments.Create;
using WebApp.Application.Features.Priorities.Create;
using WebApp.Application.Features.ProjectMembers.Create;
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
        services.AddScoped<ITaskCreatedHandler, CreateTaskCreatedActivity>();
        services.AddScoped<ICommentCreatedHandler, CreateTaskCommentedActivity>();
        services.AddScoped<ICommentDeletedHandler, DeleteActivityOnCommentDeleted>();
        return services;
    }
}
