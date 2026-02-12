using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed class StatusChangedHydratorStrategy(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<JsonOptions> jsonOptions
) : IHydratorStrategy
{
    private Dictionary<ActivityId, DataBag>? activityIdToBag;
    private List<StatusId>? statusIds;
    private Dictionary<StatusId, Status>? statuses;

    public void CollectId(Activity activity)
    {
        if (activity.Data is null)
        {
            return;
        }

        var bag = new DataBag();
        if (
            activity.Data.RootElement.TryGetProperty("StatusId", out var statusIdElement)
            && statusIdElement.ValueKind == JsonValueKind.Number
            && statusIdElement.TryGetInt64(out var statusIdValue)
        )
        {
            statusIds ??= new(1);
            var statusId = new StatusId(statusIdValue);
            statusIds.Add(statusId);
            bag.StatusId = statusId;
        }
        if (
            activity.Data.RootElement.TryGetProperty("OldStatusId", out statusIdElement)
            && statusIdElement.ValueKind == JsonValueKind.Number
            && statusIdElement.TryGetInt64(out statusIdValue)
        )
        {
            statusIds ??= new(1);
            var statusId = new StatusId(statusIdValue);
            statusIds.Add(statusId);
            bag.OldStatusId = statusId;
        }

        activityIdToBag ??= new(1);
        activityIdToBag[activity.Id] = bag;
    }

    public async Task QueryAsync(CancellationToken ct)
    {
        if (statusIds is null || statusIds.Count == 0)
        {
            return;
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        statuses = await db
            .Statuses.Where(a => statusIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public Activity Hydrate(Activity activity)
    {
        if (
            statuses is null
            || activityIdToBag is null
            || !activityIdToBag.TryGetValue(activity.Id, out var bag)
        )
        {
            return activity;
        }

        var status = bag.StatusId.HasValue ? statuses.GetValueOrDefault(bag.StatusId.Value) : null;
        var oldStatus = bag.OldStatusId.HasValue
            ? statuses.GetValueOrDefault(bag.OldStatusId.Value)
            : null;
        return activity with
        {
            Data = JsonSerializer.SerializeToDocument(
                new { Status = status, OldStatus = oldStatus },
                jsonOptions.Value.SerializerOptions
            ),
        };
    }

    private sealed class DataBag
    {
        public StatusId? StatusId;
        public StatusId? OldStatusId;
    }
}
