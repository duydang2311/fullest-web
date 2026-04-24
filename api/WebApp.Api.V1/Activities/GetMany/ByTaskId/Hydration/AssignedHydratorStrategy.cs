using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed class AssignedHydratorStrategy(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<JsonOptions> jsonOptions
) : IHydratorStrategy
{
    private Dictionary<ActivityId, UserId>? activityIdToAssigneeId;
    private HashSet<UserId>? assigneeIds;
    private Dictionary<UserId, User>? assignees;

    public void CollectId(Activity activity)
    {
        if (activity.Metadata is null)
        {
            return;
        }

        using var metadata = JsonDocument.Parse(activity.Metadata);
        if (
            metadata.RootElement.TryGetProperty("assigneeId", out var assigneeIdElement)
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
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        assignees = await db
            .Users.Where(a => assigneeIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public JsonObject? HydrateMetadata(Activity activity)
    {
        if (
            assignees is null
            || activityIdToAssigneeId is null
            || !activityIdToAssigneeId.TryGetValue(activity.Id, out var assigneeId)
            || !assignees.TryGetValue(assigneeId, out var assignee)
        )
        {
            return null;
        }
        return new JsonObject
        {
            ["assignee"] = JsonSerializer.SerializeToNode(
                assignee,
                jsonOptions.Value.SerializerOptions
            ),
        };
    }
}
