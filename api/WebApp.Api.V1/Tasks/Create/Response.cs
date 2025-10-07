using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Tasks.Create;

public sealed record Response(TaskId Id, long PublicId);
