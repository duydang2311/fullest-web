using WebApp.Domain.Entities;

namespace WebApp.Domain.Events;

public sealed record ProjectCreated(ProjectId ProjectId, UserId CreatorId);
