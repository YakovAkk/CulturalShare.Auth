using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Services.DependencyInjection;
using CulturalShare.Auth.Repositories.DependencyInjection;
using Serilog.Core;
using CulturalShare.Common.Helper.Constants;

namespace CulturalShare.Auth.API.Configuration;

public class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHeaderPropagation(options => options.Headers.Add(LoggingConsts.CorrelationIdHeaderName));

        builder.Services.AddControllers();
        builder.Services.AddAuthServices();
        builder.Services.AddAuthRepositories();
        builder.Services.AddAuthConfiguration(builder.Configuration);

        logger.Information($"{nameof(ApplicationServiceInstaller)} installed.");
    }
}
