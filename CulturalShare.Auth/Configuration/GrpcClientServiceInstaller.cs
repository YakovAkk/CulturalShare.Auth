using Serilog.Core;

using CulturalShare.Auth.API.Configuration.Base;

namespace CulturalShare.Auth.API.Configuratio;

public class GrpcClientServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddGrpc();

        logger.Information($"{nameof(GrpcClientServiceInstaller)} installed.");
    }
}
