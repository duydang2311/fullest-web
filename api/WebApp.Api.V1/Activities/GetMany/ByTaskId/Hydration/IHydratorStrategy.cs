using System.Text.Json.Nodes;
using WebApp.Domain.Entities;

namespace WebApp.Api.V1.Activities.GetMany.ByTaskId.Hydration;

public interface IHydratorStrategy
{
    void CollectId(Activity activity);
    Task QueryAsync(CancellationToken ct);
    JsonObject? HydrateMetadata(Activity activity);
}
