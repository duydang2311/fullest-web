using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Projects.Patch;

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
            .Projects.Where(a => a.Id == context.Request.ProjectId && a.DeletedTime == null)
            .Select(a => (ProjectId?)a.Id)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (!projectId.HasValue)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct);
            return;
        }

        var canPatch = await authorizer
            .HasProjectPermissionAsync(
                context.Request.CallerId,
                context.Request.ProjectId,
                Permit.UpdateProject,
                ct
            )
            .ConfigureAwait(false);

        if (!canPatch)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct);
        }
    }
}
