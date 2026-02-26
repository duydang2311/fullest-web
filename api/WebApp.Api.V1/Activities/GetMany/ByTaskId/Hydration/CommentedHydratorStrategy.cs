using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed class CommentedHydratorStrategy(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<JsonOptions> jsonOptions
) : IHydratorStrategy
{
    private Dictionary<ActivityId, CommentId>? activityIdToCommentId;
    private HashSet<CommentId>? commentIds;
    private Dictionary<CommentId, Comment>? comments;

    public void CollectId(Activity activity)
    {
        if (activity.Metadata is null)
        {
            return;
        }

        using var metadata = JsonDocument.Parse(activity.Metadata);
        if (
            metadata.RootElement.TryGetProperty("CommentId", out var commentIdElement)
            && commentIdElement.TryGetInt64(out var commentIdValue)
        )
        {
            commentIds ??= new(1);
            activityIdToCommentId ??= new(1);
            var commentId = new CommentId(commentIdValue);
            commentIds.Add(commentId);
            activityIdToCommentId[activity.Id] = commentId;
        }
    }

    public async Task QueryAsync(CancellationToken ct)
    {
        if (commentIds is null || commentIds.Count == 0)
        {
            return;
        }

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        comments = await db
            .Comments.Where(a => commentIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public JsonObject? HydrateMetadata(Activity activity)
    {
        if (
            comments is null
            || activityIdToCommentId is null
            || !activityIdToCommentId.TryGetValue(activity.Id, out var commentId)
            || !comments.TryGetValue(commentId, out var comment)
        )
        {
            return null;
        }

        return new JsonObject
        {
            ["comment"] = JsonSerializer.SerializeToNode(
                comment,
                jsonOptions.Value.SerializerOptions
            ),
        };
    }
}
