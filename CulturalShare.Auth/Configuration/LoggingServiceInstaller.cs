using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Common.Helper.Constants;
using Serilog;
using Serilog.Core;


namespace CulturalShare.Auth.API.Configuration
{
    public class LoggingServiceInstaller : IServiceInstaller
    {
        public void Install(WebApplicationBuilder builder, Logger logger)
        {
            builder.Host.UseSerilog((context, config) =>
            {
                var configuration = builder.Configuration;

                config.Enrich.WithCorrelationIdHeader(LoggingConsts.CorrelationIdHeaderName);
                config.Enrich.WithProperty(LoggingConsts.Environment, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
                config.ReadFrom.Configuration(configuration);
            });

            logger.Information($"{nameof(LoggingServiceInstaller)} installed.");
        }
    }
}
