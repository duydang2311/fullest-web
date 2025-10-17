using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;

namespace WebApp.Api.Serialization;

public class ConfigureJsonOptions(
    INumberEncoder numberEncoder,
    IProjectionService projectionService,
    IHttpContextAccessor httpContextAccessor
) : IConfigureOptions<JsonOptions>
{
    public void Configure(JsonOptions options)
    {
        options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.SerializerOptions.Converters.Add(new EntityIdJsonConverter<UserId>(numberEncoder));
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<ProjectId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<NamespaceId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<PermissionId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(new EntityIdJsonConverter<RoleId>(numberEncoder));
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<UserSessionId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<UserAuthId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<ProjectMemberId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(new EntityIdJsonConverter<TagId>(numberEncoder));
        options.SerializerOptions.Converters.Add(new EntityIdJsonConverter<TaskId>(numberEncoder));
        options.SerializerOptions.Converters.Add(new EntityIdJsonConverter<LabelId>(numberEncoder));
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<StatusId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<PriorityId>(numberEncoder)
        );
        options.SerializerOptions.Converters.Add(
            new EntityIdJsonConverter<CommentId>(numberEncoder)
        );

        options.SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
        );
        options.SerializerOptions.Converters.Add(new ProjectableJsonConverter(projectionService));
        options.SerializerOptions.Converters.Add(new ProblemJsonConverter(httpContextAccessor));
    }
}
