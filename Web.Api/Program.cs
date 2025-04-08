using Dependency.Infranstructure.Configuration.Base;
using Dependency.Infranstructure.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using WebApi.GrpcServices;

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

app.MapGrpcService<AuthenticationGrpcService>();
app.MapGrpcService<UserGrpcService>();

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
