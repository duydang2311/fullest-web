namespace WebApp.Domain.Events;

public interface IEventHub
{
    Task PublishAsync<T>(T eventModel, CancellationToken ct);
}
