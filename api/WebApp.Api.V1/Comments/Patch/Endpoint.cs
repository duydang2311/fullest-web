using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.Patch;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Results<NotFound, NoContent>>
{
    public override void Configure()
    {
        Patch("comments/{CommentId}");
        Version(1);
    }

    public override async Task<Results<NotFound, NoContent>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        Guard.Against.Null(req.Patch);

        var query = db.Comments.Where(a => a.Id == req.CommentId);
        Action<UpdateSettersBuilder<Comment>>? updateBuilder = null;

        if (req.Patch.TryGetValue(a => a.ContentJson, out var contentJson))
        {
            updateBuilder += a => a.SetProperty(b => b.ContentJson, contentJson);
        }
        Guard.Against.Null(updateBuilder);

        var count = await query.ExecuteUpdateAsync(updateBuilder, ct).ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
