using CulturalShare.Foundation.AspNetCore.Extensions.GrpcInterceptors;
using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;

namespace Dependency.Infranstructure.Configuration;

public class GrpcClientServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddSingleton<ValidationInterceptor>();

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<ValidationInterceptor>();
        }).AddJsonTranscoding();

        logger.Information($"{nameof(GrpcClientServiceInstaller)} installed.");
    }
}
