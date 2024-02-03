using CulturalShare.Auth.API.Configuration.Base;

namespace CulturalShare.Auth.API.Configuratio;

public class GrpcClientServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();
    }
}
