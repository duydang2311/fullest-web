using System.Text.Json;
using NodaTime;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public sealed record HydratedActivity
{
    public Instant CreatedTime { get; init; }
    public ActivityId Id { get; init; }
    public UserId? ActorId { get; init; }
    public User? Actor { get; init; }
    public ProjectId? ProjectId { get; init; }
    public Project? Project { get; init; }
    public TaskId? TaskId { get; init; }
    public TaskEntity? Task { get; init; }
    public ActivityKind Kind { get; init; }
    public JsonElement? Metadata { get; init; }
}
