using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed class UnassignedHydratorStrategy(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<JsonOptions> jsonOptions
) : IHydratorStrategy
{
    private Dictionary<ActivityId, UserId>? activityIdToAssigneeId;
    private List<UserId>? assigneeIds;
    private Dictionary<UserId, User>? assignees;

    public void CollectId(Activity activity)
    {
        if (
            activity.Data is not null
            && activity.Data.RootElement.TryGetProperty("AssigneeId", out var assigneeIdElement)
            && assigneeIdElement.TryGetInt64(out var assigneeIdValue)
        )
        {
            assigneeIds ??= new(1);
            activityIdToAssigneeId ??= new(1);
            var assigneeId = new UserId(assigneeIdValue);
            assigneeIds.Add(assigneeId);
            activityIdToAssigneeId[activity.Id] = assigneeId;
        }
    }

    public async Task QueryAsync(CancellationToken ct)
    {
        if (assigneeIds is null || assigneeIds.Count == 0)
        {
            return;
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        assignees = await db
            .Users.Where(a => assigneeIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public Activity Hydrate(Activity activity)
    {
        if (
            assignees is null
            || activityIdToAssigneeId is null
            || !activityIdToAssigneeId.TryGetValue(activity.Id, out var assigneeId)
            || !assignees.TryGetValue(assigneeId, out var assignee)
        )
        {
            return activity;
        }
        return activity with
        {
            Data = JsonSerializer.SerializeToDocument(
                new { Assignee = assignee },
                jsonOptions.Value.SerializerOptions
            ),
        };
    }
}
