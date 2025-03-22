using Serilog.Core;
using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Foundation.AspNetCore.Extensions.GrpcInterceptors;

namespace CulturalShare.Auth.API.Configuratio;

public class GrpcClientServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddSingleton<ValidationInterceptor>();

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionHandlerGRPCInterceptor>();
            options.Interceptors.Add<ValidationInterceptor>();
        }).AddJsonTranscoding();

        logger.Information($"{nameof(GrpcClientServiceInstaller)} installed.");
    }
}
