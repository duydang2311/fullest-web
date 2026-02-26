using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed class ActivityHydrator(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<JsonOptions> jsonOptions
)
{
    private static readonly Dictionary<
        ActivityKind,
        Func<IServiceScopeFactory, IOptions<JsonOptions>, IHydratorStrategy>
    > stratInits = new()
    {
        [ActivityKind.Commented] = (serviceScopeFactory, jsonOptions) =>
            new CommentedHydratorStrategy(serviceScopeFactory, jsonOptions),
        [ActivityKind.StatusChanged] = (serviceScopeFactory, jsonOptions) =>
            new StatusChangedHydratorStrategy(serviceScopeFactory, jsonOptions),
        [ActivityKind.PriorityChanged] = (serviceScopeFactory, jsonOptions) =>
            new PriorityChangedHydratorStrategy(serviceScopeFactory, jsonOptions),
        [ActivityKind.Assigned] = (serviceScopeFactory, jsonOptions) =>
            new AssignedHydratorStrategy(serviceScopeFactory, jsonOptions),
        [ActivityKind.Unassigned] = (serviceScopeFactory, jsonOptions) =>
            new UnassignedHydratorStrategy(serviceScopeFactory, jsonOptions),
    };

    public async Task<List<HydratedActivity>> GetHydratedActivitiesAsync(
        List<Activity> activities,
        CancellationToken ct
    )
    {
        var strats = new Dictionary<ActivityKind, IHydratorStrategy>(stratInits.Count);
        foreach (var activity in activities)
        {
            if (!strats.TryGetValue(activity.Kind, out var strat))
            {
                if (!stratInits.TryGetValue(activity.Kind, out var init))
                {
                    continue;
                }
                strat = init(serviceScopeFactory, jsonOptions);
                strats[activity.Kind] = strat;
            }
            strat.CollectId(activity);
        }

        await Task.WhenAll(strats.Select(a => a.Value.QueryAsync(ct))).ConfigureAwait(false);
        // var projects =
        //     hydrateActivityOptions?.SelectProject is not null
        //     && !string.IsNullOrEmpty(hydrateActivityOptions.SelectProject)
        //         ? await GetProjectsAsync(activities, hydrateActivityOptions.SelectProject, ct)
        //             .ConfigureAwait(false)
        //         : null;
        // var tasks =
        //     hydrateActivityOptions?.SelectTask is not null
        //     && !string.IsNullOrEmpty(hydrateActivityOptions.SelectTask)
        //         ? await GetTasksAsync(activities, hydrateActivityOptions.SelectTask, ct)
        //             .ConfigureAwait(false)
        //         : null;

        var results = new List<HydratedActivity>(activities.Count);
        foreach (var a in activities)
        {
            var metadata = strats.TryGetValue(a.Kind, out var strat)
                ? strat.HydrateMetadata(a)
                : null;
            // if (
            //     projects is not null
            //     && a.ProjectId.HasValue
            //     && projects.TryGetValue(a.ProjectId.Value, out var project)
            // )
            // {
            //     Guard.Against.Null(hydrateActivityOptions?.SelectProject);
            //     using var stream = new MemoryStream();
            //     using var writer = new Utf8JsonWriter(stream);
            //     FieldProjector.Project(
            //         writer,
            //         project,
            //         hydrateActivityOptions.SelectProject,
            //         jsonOptions.Value.SerializerOptions
            //     );
            //     writer.Flush();
            //     stream.Position = 0;
            //     metadata ??= [];
            //     metadata["context"] = JsonNode.Parse(stream);
            // }
            // if (
            //     tasks is not null
            //     && a.TaskId.HasValue
            //     && tasks.TryGetValue(a.TaskId.Value, out var task)
            // )
            // {
            //     Guard.Against.Null(hydrateActivityOptions?.SelectTask);
            //     using var stream = new MemoryStream();
            //     using var writer = new Utf8JsonWriter(stream);
            //     FieldProjector.Project(
            //         writer,
            //         task,
            //         hydrateActivityOptions.SelectTask,
            //         jsonOptions.Value.SerializerOptions
            //     );
            //     writer.Flush();
            //     stream.Position = 0;
            //     metadata ??= [];
            //     metadata["resource"] = JsonNode.Parse(stream);
            // }
            results.Add(
                new HydratedActivity
                {
                    CreatedTime = a.CreatedTime,
                    Id = a.Id,
                    ActorId = a.ActorId,
                    Actor = a.Actor,
                    ProjectId = a.ProjectId,
                    Project = a.Project,
                    TaskId = a.TaskId,
                    Task = a.Task,
                    Kind = a.Kind,
                    Metadata = JsonSerializer.SerializeToElement(
                        metadata,
                        jsonOptions.Value.SerializerOptions
                    ),
                }
            );
        }
        return results;
    }

    // private async Task<Dictionary<ProjectId, Project>> GetProjectsAsync(
    //     List<Activity> activities,
    //     string fields,
    //     CancellationToken ct
    // )
    // {
    //     var projectIds = activities
    //         .Where(a => a.ProjectId.HasValue)
    //         .Select(a => a.ProjectId)
    //         .Cast<ProjectId>()
    //         .ToHashSet();
    //     await using var scope = serviceScopeFactory.CreateAsyncScope();
    //     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //     var projects = await db
    //         .Projects.Where(a => projectIds.Contains(a.Id))
    //         .Select(FieldProjector.Project<Project>(fields))
    //         .ToDictionaryAsync(a => a.Id, ct)
    //         .ConfigureAwait(false);
    //     return projects;
    // }

    // private async Task<Dictionary<TaskId, TaskEntity>> GetTasksAsync(
    //     List<Activity> activities,
    //     string fields,
    //     CancellationToken ct
    // )
    // {
    //     var taskIds = activities
    //         .Where(a => a.TaskId.HasValue)
    //         .Select(a => a.TaskId)
    //         .Cast<TaskId>()
    //         .ToHashSet();
    //     await using var scope = serviceScopeFactory.CreateAsyncScope();
    //     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //     var tasks = await db
    //         .Tasks.Where(a => taskIds.Contains(a.Id))
    //         .Select(FieldProjector.Project<TaskEntity>(fields))
    //         .ToDictionaryAsync(a => a.Id, ct)
    //         .ConfigureAwait(false);
    //     return tasks;
    // }
}
