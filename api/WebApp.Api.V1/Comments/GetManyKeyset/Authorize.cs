using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.GetManyKeyset;

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
        var projectId = await db
            .Tasks.Where(a => a.Id == context.Request.TaskId)
            .Select(a => (ProjectId?)a.ProjectId)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
        if (projectId is null)
        {
            await context.HttpContext.Response.SendNotFoundAsync(ct).ConfigureAwait(false);
            return;
        }

        var canRead = await authorizer.HasProjectPermissionAsync(
            context.Request.CallerId,
            projectId.Value,
            Permit.ReadComment,
            ct
        );
        if (!canRead)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
        }
    }
}
