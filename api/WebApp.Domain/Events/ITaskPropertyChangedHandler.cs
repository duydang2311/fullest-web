namespace WebApp.Domain.Events;

public interface ITaskPropertyChangedHandler : IEventHandler<TaskPropertyChanged>
{
    bool CanHandle(TaskPropertyChanged changed);
}
