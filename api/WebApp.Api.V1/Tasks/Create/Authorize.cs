using FastEndpoints;
using WebApp.Domain.Constants;
using WebApp.Infrastructure.AccessControl;

namespace WebApp.Api.V1.Tasks.Create;

public sealed class Authorize : IPreProcessor<Request>
{
    public async Task PreProcessAsync(IPreProcessorContext<Request> context, CancellationToken ct)
    {
        if (context.Request is null || context.HasValidationFailures)
        {
            return;
        }

        var authorizer = context.HttpContext.Resolve<IAuthorizer>();
        var canCreate = await authorizer.HasProjectPermissionAsync(
            context.Request.CallerId,
            context.Request.ProjectId,
            Permit.CreateTask,
            ct
        );
        if (!canCreate)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct).ConfigureAwait(false);
        }
    }
}
