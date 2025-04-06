using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using static Service.Handlers.MediatRCommands;

namespace Dependency.Infranstructure.Configuration;

public class MediatRServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddMediatR(config =>
        {
            config.Lifetime = ServiceLifetime.Scoped;
            config.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
        });

        logger.Information($"{nameof(MediatRServiceInstaller)} installed.");
    }
}
