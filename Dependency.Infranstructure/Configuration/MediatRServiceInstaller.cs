using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Service.Services.Handlers.Command;

namespace Dependency.Infranstructure.Configuration;

public class MediatRServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddMediatR(config =>
        {
            config.Lifetime = ServiceLifetime.Scoped;
            config.RegisterServicesFromAssembly(typeof(GetServiceTokenHandler).Assembly);
        });

        logger.Information($"{nameof(MediatRServiceInstaller)} installed.");
    }
}
