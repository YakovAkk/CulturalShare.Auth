using Serilog.Core;
using CulturalShare.Auth.API.Configuration.Base;

namespace CulturalShare.Auth.API.Configuration;

public class HealthCheckServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddHealthChecks()
           .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"), name: "AuthDB");

        logger.Information($"{nameof(HealthCheckServiceInstaller)} installed.");
    }
}
