using System.Text.Json;
using NodaTime;

namespace WebApp.Domain.Entities;

public sealed record Activity : IDisposable
{
    public Instant CreatedTime { get; init; }
    public ActivityId Id { get; init; }
    public TaskId? TaskId { get; init; }
    public TaskEntity? Task { get; init; }
    public UserId? ActorId { get; init; }
    public User? Actor { get; init; }
    public ActivityKind Kind { get; init; }
    public JsonDocument? Data { get; init; }

    public void Dispose() => Data?.Dispose();
}
