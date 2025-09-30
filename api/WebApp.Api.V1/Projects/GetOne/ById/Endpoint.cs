using FastEndpoints;

namespace WebApp.Api.V1.Projects.GetOne.ById;

public sealed class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("projects/{ProjectId}");
        Version(1);
    }
}
