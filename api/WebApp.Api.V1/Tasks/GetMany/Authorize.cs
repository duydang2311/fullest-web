using FastEndpoints;
using WebApp.Domain.Constants;
using WebApp.Infrastructure.AccessControl;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.GetMany;

public sealed class Authorize : IPreProcessor<Request>
{
    public async Task PreProcessAsync(IPreProcessorContext<Request> context, CancellationToken ct)
    {
        if (context.Request is null || context.HasValidationFailures)
        {
            return;
        }

        if (!context.Request.ProjectId.HasValue)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
            return;
        }

        var authorizer = context.HttpContext.Resolve<IAuthorizer>();
        var canRead = await authorizer
            .HasProjectPermissionAsync(
                context.Request.CallerId,
                context.Request.ProjectId.Value,
                Permit.ReadTask,
                ct
            )
            .ConfigureAwait(false);
        if (!canRead)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
            return;
        }
    }
}
