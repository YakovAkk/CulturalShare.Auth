using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.API.DependencyInjection;
using CulturalShare.Auth.Services;
using CulturalShare.Foundation.AspNetCore.Extensions.MiddlewareClasses;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.InstallServices(logger, typeof(IServiceInstaller).Assembly);
var app = builder.Build();

if (app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AuthenticationService>();

app.UseMiddleware<HandlingExceptionsMiddleware>();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsEnvironment("Test"))
{
    app.MapControllers();
}

app.UseSerilogRequestLogging();

logger.Information($"Env: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")} Running App...");
app.Run();
logger.Information("App finished.");
