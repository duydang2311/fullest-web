namespace WebApp.Domain.Events;

public interface ITaskCreatedHandler
{
    Task HandleAsync(TaskCreated created, CancellationToken ct);
}
