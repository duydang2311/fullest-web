using FastEndpoints;
using FluentValidation;
using Slugify;
using WebApp.Api;
using WebApp.Api.Middlewares;
using WebApp.Api.Security;
using WebApp.Api.Serialization;
using WebApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddDataGroup()
    .AddCodecsGroup()
    .AddHashingGroup()
    .AddAccessControlGroup()
    .AddEventHandlersGroup()
    .AddStorageGroup()
    .AddIntegrationGroup();
builder.Services.AddSingleton<ISlugHelper, SlugHelper>();

builder.Services.AddProblemDetails(a =>
{
    a.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance ??=
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHybridCache();
builder
    .Services.AddAuthentication("Session")
    .AddScheme<SessionAuthSchemeOptions, SessionAuthSchemeHandler>("Session", null)
    .AddScheme<ServiceAuthSchemeOptions, ServiceAuthSchemeHandler>("Service", null);
builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<JsonExceptionHandler>();
builder.Services.ConfigureOptions<ConfigureJsonOptions>();
builder.Services.AddFastEndpoints(FastEndpointsConfiguration.ConfigureDiscovery);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(options =>
{
    FastEndpointsConfiguration.ConfigureFastEndpoints(app, options);
});

ValidatorOptions.Global.LanguageManager.Enabled = false;

// var rsa = RSA.Create(2048);
// Console.WriteLine(rsa.ExportPkcs8PrivateKeyPem());
// Console.WriteLine(rsa.ExportSubjectPublicKeyInfoPem());
// return 0;
// Console.WriteLine(
//     JwtHelper.CreateToken(
//         app.Services.GetRequiredService<
//             IOptions<IntegrationKeysOptions>
//         >().Value.AssetWorkers.PrivateKeyPem,
//         iss: "api",
//         aud: "workers-assets",
//         iat: DateTime.UtcNow,
//         nbf: DateTime.UtcNow,
//         exp: DateTime.UtcNow.AddYears(1)
//     )
// );
// return 0;

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
await using (var scope = scopeFactory.CreateAsyncScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<SeedDatabase>();
    await seed.EnsureRolesAsync().ConfigureAwait(false);
}

await app.RunAsync().ConfigureAwait(false);
