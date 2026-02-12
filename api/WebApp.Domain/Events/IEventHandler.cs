namespace WebApp.Domain.Events;

public interface IEventHandler<T>
{
    Task HandleAsync(T eventModel, CancellationToken ct);
}
