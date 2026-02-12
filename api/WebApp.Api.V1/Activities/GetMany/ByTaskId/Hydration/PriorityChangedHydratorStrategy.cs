using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed class PriorityChangedHydratorStrategy(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<JsonOptions> jsonOptions
) : IHydratorStrategy
{
    private Dictionary<ActivityId, DataBag>? activityIdToBag;
    private List<PriorityId>? priorityIds;
    private Dictionary<PriorityId, Priority>? priorities;

    public void CollectId(Activity activity)
    {
        if (activity.Data is null)
        {
            return;
        }

        var bag = new DataBag();
        if (
            activity.Data.RootElement.TryGetProperty("PriorityId", out var priorityIdElement)
            && priorityIdElement.ValueKind == JsonValueKind.Number
            && priorityIdElement.TryGetInt64(out var priorityIdValue)
        )
        {
            priorityIds ??= new(1);
            var priorityId = new PriorityId(priorityIdValue);
            priorityIds.Add(priorityId);
            bag.PriorityId = priorityId;
        }
        if (
            activity.Data.RootElement.TryGetProperty("OldPriorityId", out priorityIdElement)
            && priorityIdElement.ValueKind == JsonValueKind.Number
            && priorityIdElement.TryGetInt64(out priorityIdValue)
        )
        {
            priorityIds ??= new(1);
            var priorityId = new PriorityId(priorityIdValue);
            priorityIds.Add(priorityId);
            bag.OldPriorityId = priorityId;
        }

        activityIdToBag ??= new(1);
        activityIdToBag[activity.Id] = bag;
    }

    public async Task QueryAsync(CancellationToken ct)
    {
        if (priorityIds is null || priorityIds.Count == 0)
        {
            return;
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        priorities = await db
            .Priorities.Where(a => priorityIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public Activity Hydrate(Activity activity)
    {
        if (
            priorities is null
            || activityIdToBag is null
            || !activityIdToBag.TryGetValue(activity.Id, out var bag)
        )
        {
            return activity;
        }

        var priority = bag.PriorityId.HasValue
            ? priorities.GetValueOrDefault(bag.PriorityId.Value)
            : null;
        var oldPriority = bag.OldPriorityId.HasValue
            ? priorities.GetValueOrDefault(bag.OldPriorityId.Value)
            : null;
        return activity with
        {
            Data = JsonSerializer.SerializeToDocument(
                new { Priority = priority, OldPriority = oldPriority },
                jsonOptions.Value.SerializerOptions
            ),
        };
    }

    private sealed class DataBag
    {
        public PriorityId? PriorityId;
        public PriorityId? OldPriorityId;
    }
}
