using Serilog.Core;
using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Common.Helper.GrpcInterceptors;

namespace CulturalShare.Auth.API.Configuratio;

public class GrpcClientServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services
            .AddGrpc(c => c.Interceptors.Add<ExceptionHandlerGRPCInterceptor>());

        logger.Information($"{nameof(GrpcClientServiceInstaller)} installed.");
    }
}
