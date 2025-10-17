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

        config.Binding.ValueParserForEntityId<UserId>(numberEncoder);
        config.Binding.ValueParserForEntityId<ProjectId>(numberEncoder);
        config.Binding.ValueParserForEntityId<PermissionId>(numberEncoder);
        config.Binding.ValueParserForEntityId<UserSessionId>(numberEncoder);
        config.Binding.ValueParserForEntityId<RoleId>(numberEncoder);
        config.Binding.ValueParserForEntityId<UserAuthId>(numberEncoder);
        config.Binding.ValueParserForEntityId<ProjectMemberId>(numberEncoder);
        config.Binding.ValueParserForEntityId<NamespaceId>(numberEncoder);
        config.Binding.ValueParserForEntityId<TagId>(numberEncoder);
        config.Binding.ValueParserForEntityId<TaskId>(numberEncoder);
        config.Binding.ValueParserForEntityId<LabelId>(numberEncoder);
        config.Binding.ValueParserForEntityId<StatusId>(numberEncoder);
        config.Binding.ValueParserForEntityId<PriorityId>(numberEncoder);
        config.Binding.ValueParserForEntityId<CommentId>(numberEncoder);
    }
}

public static class BindingOptionsExtensions
{
    public static void ValueParserForEntityId<T>(
        this BindingOptions options,
        INumberEncoder numberEncoder
    )
        where T : IEntityId<long>, new()
    {
        options.ValueParserFor<T>(values =>
        {
            if (values.Count == 0)
            {
                return new ParseResult(false, null);
            }
            return ParseInternal<T>(values[0], numberEncoder);
        });
        options.ValueParserFor(
            typeof(Nullable<>).MakeGenericType(typeof(T)),
            values =>
            {
                if (values.Count == 0)
                {
                    return new ParseResult(true, null);
                }
                return ParseInternal<T>(values[0], numberEncoder);
            }
        );
    }

    private static ParseResult ParseInternal<T>(string? value, INumberEncoder numberEncoder)
        where T : IEntityId<long>, new()
    {
        if (string.IsNullOrEmpty(value) || !numberEncoder.TryDecode(value, out long decodedValue))
        {
            return new ParseResult(false, default);
        }
        return new ParseResult(true, new T { Value = decodedValue });
    }
}
