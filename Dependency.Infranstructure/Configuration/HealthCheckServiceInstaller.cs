using CulturalShare.Foundation.EntironmentHelper.EnvHelpers;
using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;

namespace Dependency.Infranstructure.Configuration;

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
