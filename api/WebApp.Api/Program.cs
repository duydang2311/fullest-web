using FastEndpoints;
using FluentValidation;
using Slugify;
using WebApp.Api;
using WebApp.Api.Middlewares;
using WebApp.Api.Security;
using WebApp.Api.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataGroup().AddProjectionGroup().AddCodecsGroup().AddHashingGroup();
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
    .Services.AddAuthentication()
    .AddScheme<SessionAuthSchemeOptions, SessionAuthSchemeHandler>("Session", null);
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

app.Run();
