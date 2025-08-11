using WebApp.Api.Common.Codecs;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodecsGroup(this IServiceCollection services)
    {
        services
            .AddOptions<NumberEncoderOptions>()
            .BindConfiguration(NumberEncoderOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<INumberEncoder, NumberEncoder>();
        return services;
    }
}
