using WebApp.Application.Common;
using WebApp.Application.Data;
using WebApp.Domain.Commands;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Comments.Create;

public sealed class CreateCommentHandler(
    BaseDbContext db,
    IEnumerable<ICommentCreatedHandler> commentCreatedHandlers
) : ICreateCommentHandler
{
    public async Task<Comment> HandleAsync(CreateComment command, CancellationToken ct)
    {
        var (json, preview) = TextDocumentHelper.ParseDocumentPreview(command.ContentJson);
        var comment = new Comment
        {
            TaskId = command.TaskId,
            AuthorId = command.AuthorId,
            ContentJson = json,
            ContentPreview = preview,
        };
        await db.AddAsync(comment, ct).ConfigureAwait(false);

        var commentCreated = new CommentCreated(command.ProjectId, comment);
        foreach (var handler in commentCreatedHandlers)
        {
            await handler.HandleAsync(commentCreated, ct).ConfigureAwait(false);
        }

        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        return comment;
    }
}
