using System.Text.Json;
using System.Text.Json.Nodes;
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
    private HashSet<PriorityId>? priorityIds;
    private Dictionary<PriorityId, Priority>? priorities;

    public void CollectId(Activity activity)
    {
        if (activity.Metadata is null)
        {
            return;
        }

        using var metadata = JsonDocument.Parse(activity.Metadata);
        var bag = new DataBag();
        if (
            metadata.RootElement.TryGetProperty("priorityId", out var priorityIdElement)
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
            metadata.RootElement.TryGetProperty("oldPriorityId", out priorityIdElement)
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
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        priorities = await db
            .Priorities.Where(a => priorityIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public JsonObject? HydrateMetadata(Activity activity)
    {
        if (
            priorities is null
            || activityIdToBag is null
            || !activityIdToBag.TryGetValue(activity.Id, out var bag)
        )
        {
            return null;
        }

        var priority = bag.PriorityId.HasValue
            ? priorities.GetValueOrDefault(bag.PriorityId.Value)
            : null;
        var oldPriority = bag.OldPriorityId.HasValue
            ? priorities.GetValueOrDefault(bag.OldPriorityId.Value)
            : null;

        return new JsonObject
        {
            ["priority"] = JsonSerializer.SerializeToNode(
                priority,
                jsonOptions.Value.SerializerOptions
            ),
            ["oldPriority"] = JsonSerializer.SerializeToNode(
                oldPriority,
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
