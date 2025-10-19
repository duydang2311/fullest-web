using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Comments.Create;

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

        var canCreate = await authorizer
            .HasProjectPermissionAsync(
                context.Request.CallerId,
                projectId.Value,
                Permit.CreateComment,
                ct
            )
            .ConfigureAwait(false);
        if (!canCreate)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
        }
    }
}
