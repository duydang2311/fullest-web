using Microsoft.Extensions.DependencyInjection;
using WebApp.Domain.Events;

namespace WebApp.Application.Events;

public sealed class EventHub(IServiceProvider serviceProvider) : IEventHub
{
    public async Task PublishAsync<T>(T eventModel, CancellationToken ct)
    {
        foreach (var handler in serviceProvider.GetServices<IEventHandler<T>>())
        {
            await handler.HandleAsync(eventModel, ct).ConfigureAwait(false);
        }
    }
}
