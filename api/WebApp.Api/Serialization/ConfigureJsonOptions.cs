using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Projection;

namespace WebApp.Api.Serialization;

public class ConfigureJsonOptions(
    INumberEncoder numberEncoder,
    IProjectionService projectionService
) : IConfigureOptions<JsonOptions>
{
    public void Configure(JsonOptions options)
    {
        options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.SerializerOptions.Converters.Add(new UserIdJsonConverter(numberEncoder));
        options.SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, true)
        );
        options.SerializerOptions.Converters.Add(new ProjectableJsonConverter(projectionService));
    }
}
