using WebApp.Domain.Entities;

namespace WebApp.Domain.Events;

public sealed record CommentCreated(ProjectId ProjectId, Comment Comment);
