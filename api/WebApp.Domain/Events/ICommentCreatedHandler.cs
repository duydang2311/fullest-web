namespace WebApp.Domain.Events;

public interface ICommentCreatedHandler
{
    Task HandleAsync(CommentCreated created, CancellationToken ct);
}
