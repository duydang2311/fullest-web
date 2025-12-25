using System.Security.Cryptography;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Options;
using Slugify;
using WebApp.Api;
using WebApp.Api.Middlewares;
using WebApp.Api.Security;
using WebApp.Api.Serialization;
using WebApp.Infrastructure.Data;
using WebApp.Infrastructure.Integrations;
using WebApp.Infrastructure.Jwts;

if (args.Contains("--gen-keypair"))
{
    var rsa = RSA.Create(2048);
    Console.WriteLine(
        "PRIVATE KEY: " + Environment.NewLine + rsa.ExportPkcs8PrivateKeyPem().Replace("\n", "\\n")
    );
    Console.WriteLine(
        "PUBLIC KEY: "
            + Environment.NewLine
            + rsa.ExportSubjectPublicKeyInfoPem().Replace("\n", "\\n")
    );
    return 0;
}

if (args.Contains("--gen-alphabet"))
{
    Span<char> alphabet =
    [
        'a',
        'b',
        'c',
        'd',
        'e',
        'f',
        'g',
        'h',
        'i',
        'j',
        'k',
        'l',
        'm',
        'n',
        'o',
        'p',
        'q',
        'r',
        's',
        't',
        'u',
        'v',
        'w',
        'x',
        'y',
        'z',
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
        '-',
        '_',
    ];
    RandomNumberGenerator.Shuffle(alphabet);
    Console.WriteLine(alphabet.ToString());
    return 0;
}

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

if (args.Contains("--workers-assets-jwt"))
{
    Console.WriteLine(
        JwtHelper.CreateToken(
            app.Services.GetRequiredService<
                IOptions<IntegrationKeysOptions>
            >().Value.AssetWorkers.PrivateKeyPem,
            iss: "api",
            aud: "workers-assets",
            iat: DateTime.UtcNow,
            nbf: DateTime.UtcNow,
            exp: DateTime.UtcNow.AddYears(1)
        )
    );
    return 0;
}

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

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
await using (var scope = scopeFactory.CreateAsyncScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<SeedDatabase>();
    await seed.EnsureRolesAsync().ConfigureAwait(false);
}

await app.RunAsync().ConfigureAwait(false);

return 0;
