using Serilog.Core;
using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Common.Helper.EnvHelpers;

namespace CulturalShare.Auth.API.Configuration;

public class HealthCheckServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var sortOutCredentialsHelper = new SortOutCredentialsHelper(builder.Configuration);

        builder.Services.AddHealthChecks()
           .AddNpgSql(sortOutCredentialsHelper.GetPostgresConnectionString(), name: "AuthDB");

        logger.Information($"{sortOutCredentialsHelper.GetPostgresConnectionString()} PostgresConnectionString.");
        logger.Information($"{nameof(HealthCheckServiceInstaller)} installed.");
    }
}
 