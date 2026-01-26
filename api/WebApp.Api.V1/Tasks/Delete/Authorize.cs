using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.Delete;

public sealed class Authorize : IPreProcessor<Request>
{
    public async Task PreProcessAsync(IPreProcessorContext<Request> context, CancellationToken ct)
    {
        if (context.Request is null || context.HasValidationFailures)
        {
            return;
        }

        var authorizer = context.HttpContext.Resolve<IAuthorizer>();
        var db = context.HttpContext.Resolve<AppDbContext>();
        var task = await db
            .Tasks.Where(a => a.Id == context.Request.TaskId)
            .Select(a => new { a.ProjectId, a.AuthorId })
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (task is null)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
            return;
        }
        var canDelete = task.AuthorId == context.Request.CallerId;
        if (!canDelete)
        {
            await authorizer.HasProjectPermissionAsync(
                context.Request.CallerId,
                task.ProjectId,
                Permit.DeleteTask,
                ct
            );
        }

        if (!canDelete)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
        }
    }
}
