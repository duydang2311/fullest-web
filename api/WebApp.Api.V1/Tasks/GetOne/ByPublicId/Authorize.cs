using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Constants;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.GetOne.ByPublicId;

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
        var canRead = await db
            .Tasks.AnyAsync(
                a =>
                    a.ProjectId == context.Request.ProjectId
                    && a.PublicId == context.Request.PublicId
                    && a.AuthorId == context.Request.CallerId,
                ct
            )
            .ConfigureAwait(false);
        if (!canRead)
        {
            await authorizer.HasProjectPermissionAsync(
                context.Request.CallerId,
                context.Request.ProjectId,
                Permit.ReadTask,
                ct
            );
        }

        if (!canRead)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
        }
    }
}
