using FluentValidation;
using WebApp.Api.Middlewares;
using WebApp.Api.Serialization;
using WebApp.Api.Users.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataGroup().AddProjectionGroup().AddCodecsGroup().AddHashingGroup();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder
    .Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
    })
    .EnableApiVersionBinding();
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<JsonExceptionHandler>();
builder.Services.ConfigureOptions<ConfigureJsonOptions>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUser.RequestValidator>(
    ServiceLifetime.Singleton
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.MapUserApiV1();

app.Run();
