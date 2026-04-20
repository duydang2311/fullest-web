using WebApp.Application.Events;
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
        services.AddScoped<IEventHandler<TaskUpdated>, CreateTaskUpdatedActivities>();
        services.AddKeyedScoped<ITaskPropertyChangedHandler, CreateTaskDueTimeChangedActivity>(
            typeof(TaskDueTimeChanged)
        );
        services.AddKeyedScoped<ITaskPropertyChangedHandler, CreateTaskPriorityChangedActivity>(
            typeof(TaskPriorityChanged)
        );
        services.AddKeyedScoped<ITaskPropertyChangedHandler, CreateTaskStatusChangedActivity>(
            typeof(TaskStatusChanged)
        );
        services.AddKeyedScoped<ITaskPropertyChangedHandler, CreateTaskTitleChangedActivity>(
            typeof(TaskTitleChanged)
        );
        services.AddKeyedScoped<ITaskPropertyChangedHandler, CreateTaskDescriptionChangedActivity>(
            typeof(TaskDescriptionChanged)
        );
        AddTaskPropertyChangedHandler<TaskAssigned, CreateTaskAssignedActivity>(services);
        AddTaskPropertyChangedHandler<TaskUnassigned, CreateTaskUnassignedActivity>(services);
        return services;
    }

    public static IServiceCollection AddEventsGroup(this IServiceCollection services)
    {
        services.AddScoped<IEventHub, EventHub>();
        return services;
    }

    public static void AddTaskPropertyChangedHandler<TEvent, THandler>(IServiceCollection services)
        where THandler : class, ITaskPropertyChangedHandler, IEventHandler<TEvent>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IEventHandler<TEvent>, THandler>();
        services.AddKeyedScoped<ITaskPropertyChangedHandler, THandler>(typeof(TEvent));
    }
}
