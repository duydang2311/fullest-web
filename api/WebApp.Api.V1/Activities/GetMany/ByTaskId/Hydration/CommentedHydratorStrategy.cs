using System.Text.Json;
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
    private List<CommentId>? commentIds;
    private Dictionary<CommentId, Comment>? comments;

    public void CollectId(Activity activity)
    {
        if (
            activity.Data is not null
            && activity.Data.RootElement.TryGetProperty("CommentId", out var commentIdElement)
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
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        comments = await db
            .Comments.Where(a => commentIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct)
            .ConfigureAwait(false);
    }

    public Activity Hydrate(Activity activity)
    {
        if (
            comments is null
            || activityIdToCommentId is null
            || !activityIdToCommentId.TryGetValue(activity.Id, out var commentId)
            || !comments.TryGetValue(commentId, out var comment)
        )
        {
            return activity;
        }
        return activity with
        {
            Data = JsonSerializer.SerializeToDocument(
                new { Comment = comment },
                jsonOptions.Value.SerializerOptions
            ),
        };
    }
}
