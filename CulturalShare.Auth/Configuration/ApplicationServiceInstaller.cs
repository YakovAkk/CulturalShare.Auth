using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Services.DependencyInjection;
using CulturalShare.Auth.Repositories.DependencyInjection;

namespace CulturalShare.Auth.API.Configuration;

public class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddAuthServices();
        builder.Services.AddAuthRepositories();
        builder.Services.AddAuthConfiguration(builder.Configuration);
    }
}
