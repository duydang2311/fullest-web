using System.Linq.Expressions;
using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApp.Api.Common.Expressions;
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
        Expression<Func<SetPropertyCalls<Comment>, SetPropertyCalls<Comment>>>? setPropertyCalls =
            null;

        Console.WriteLine("Present properties: " + string.Join(',', req.Patch.PresentProperties));
        if (req.Patch.TryGetValue(a => a.ContentJson, out var contentJson))
        {
            setPropertyCalls = ExpressionHelper.Append(
                setPropertyCalls,
                a => a.SetProperty(b => b.ContentJson, contentJson)
            );
        }
        Guard.Against.Null(setPropertyCalls);

        var count = await query.ExecuteUpdateAsync(setPropertyCalls, ct).ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
