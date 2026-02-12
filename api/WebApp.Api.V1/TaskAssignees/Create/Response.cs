using WebApp.Domain.Entities;

namespace WebApp.Api.V1.TaskAssignees.Create;

public sealed record Response(TaskId TaskId, UserId UserId);
