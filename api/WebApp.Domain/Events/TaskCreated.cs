using WebApp.Domain.Entities;

namespace WebApp.Domain.Events;

public sealed record TaskCreated(TaskEntity Task);
