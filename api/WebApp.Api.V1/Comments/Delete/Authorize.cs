using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.Delete;

public sealed class Authorize : IPreProcessor<Request>
{
    public async Task PreProcessAsync(IPreProcessorContext<Request> context, CancellationToken ct)
    {
        if (context.HasValidationFailures || context.Request is null)
        {
            return;
        }

        var db = context.HttpContext.Resolve<AppDbContext>();
        var authorizer = context.HttpContext.Resolve<IAuthorizer>();
        var comment = await db
            .Comments.Where(a => a.Id == context.Request.CommentId)
            .Select(a => new { a.AuthorId, a.Task.ProjectId })
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (comment is null)
        {
            await context.HttpContext.Response.SendNotFoundAsync(ct).ConfigureAwait(false);
            return;
        }

        var canDelete =
            comment.AuthorId == context.Request.CallerId
            || await authorizer
                .HasProjectPermissionAsync(
                    context.Request.CallerId,
                    comment.ProjectId,
                    Permit.DeleteComment,
                    ct
                )
                .ConfigureAwait(false);
        if (!canDelete)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
        }
    }
}
