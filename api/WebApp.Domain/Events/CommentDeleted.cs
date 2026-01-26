using WebApp.Domain.Entities;

namespace WebApp.Domain.Events;

public sealed record CommentDeleted(CommentId CommentId);
