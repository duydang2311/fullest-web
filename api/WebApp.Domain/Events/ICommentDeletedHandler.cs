namespace WebApp.Domain.Events;

public interface ICommentDeletedHandler
{
    Task HandleAsync(CommentDeleted deleted, CancellationToken ct);
}
