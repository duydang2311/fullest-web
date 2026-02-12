namespace WebApp.Domain.Events;

public abstract class TaskPropertyChangedHandler<T> : ITaskPropertyChangedHandler
    where T : TaskPropertyChanged
{
    public bool CanHandle(TaskPropertyChanged changed) => changed is T;

    public Task HandleAsync(TaskPropertyChanged changed, CancellationToken ct) =>
        HandleAsync((T)changed, ct);

    public abstract Task HandleAsync(T changed, CancellationToken ct);
}
