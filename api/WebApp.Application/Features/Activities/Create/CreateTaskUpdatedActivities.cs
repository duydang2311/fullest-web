using Microsoft.Extensions.DependencyInjection;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Activities.Create;

public sealed class CreateTaskUpdatedActivities(IServiceProvider serviceProvider)
    : ITaskUpdatedHandler
{
    public async Task HandleAsync(TaskUpdated updated, CancellationToken ct)
    {
        foreach (var changed in updated.Changes)
        {
            var handler = serviceProvider.GetKeyedService<ITaskPropertyChangedHandler>(
                changed.GetType()
            );
            if (handler is not null)
            {
                await handler.HandleAsync(changed, ct).ConfigureAwait(false);
            }
        }
    }
}
