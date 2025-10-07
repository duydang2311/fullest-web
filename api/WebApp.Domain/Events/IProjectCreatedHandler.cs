namespace WebApp.Domain.Events;

public interface IProjectCreatedHandler
{
    Task HandleAsync(ProjectCreated created, CancellationToken ct);
}
