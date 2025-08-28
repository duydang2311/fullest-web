using System.Reflection;
using FastEndpoints;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;

namespace WebApp.Api;

public static class FastEndpointsConfiguration
{
    public static void ConfigureDiscovery(EndpointDiscoveryOptions options)
    {
        options.DisableAutoDiscovery = true;
        options.IncludeAbstractValidators = true;
        options.Assemblies = [Assembly.Load("WebApp.Api.V1")];
    }

    public static void ConfigureFastEndpoints(WebApplication app, Config config)
    {
        var numberEncoder = app.Services.GetRequiredService<INumberEncoder>();
        var projectionService = app.Services.GetRequiredService<IProjectionService>();

        config.Endpoints.RoutePrefix = "api";
        config.Versioning.Prefix = "v";
        config.Errors.ResponseBuilder = (failures, context, statusCode) =>
        {
            return new Problem(
                failures.Select(a => new ProblemError
                {
                    Field = a.PropertyName,
                    Code = a.ErrorCode ?? ErrorCodes.Json,
                })
            )
            {
                Status = statusCode,
                Instance = $"{context.Request.Method} {context.Request.Path}",
                TraceId = context.TraceIdentifier,
            };
        };

        config.Binding.ValueParserFor<UserId>(values =>
        {
            var value = values.FirstOrDefault();
            if (
                string.IsNullOrEmpty(value)
                || !numberEncoder.TryDecode(value, out long decodedValue)
            )
            {
                return new ParseResult(false, null);
            }
            return new ParseResult(true, new UserId(decodedValue));
        });
    }
}
