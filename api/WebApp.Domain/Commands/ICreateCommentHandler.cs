using WebApp.Domain.Entities;

namespace WebApp.Domain.Commands;

public interface ICreateCommentHandler
{
    Task<Comment> HandleAsync(CreateComment command, CancellationToken ct);
}
