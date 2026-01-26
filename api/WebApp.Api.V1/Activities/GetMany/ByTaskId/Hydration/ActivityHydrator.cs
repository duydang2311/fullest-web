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
        {
            ActivityKind.Commented,
            (serviceScopeFactory, jsonOptions) =>
                new CommentedHydratorStrategy(serviceScopeFactory, jsonOptions)
        },
    };

    public async Task<List<Activity>> GetHydratedActivitiesAsync(
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
        await Task.WhenAll(strats.Select(a => a.Value.QueryAsync(ct)));
        var results = new List<Activity>(activities.Count);
        foreach (var a in activities)
        {
            results.Add(strats.TryGetValue(a.Kind, out var strat) ? strat.Hydrate(a) : a);
        }
        return results;
    }
}
