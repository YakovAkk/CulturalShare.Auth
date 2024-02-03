using CulturalShare.Auth.API.Configuration.Base;

namespace CulturalShare.Auth.API.Configuration;

public class HealthCheckServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
           .AddNpgSql(builder.Configuration.GetConnectionString("AuthDB"), name: "AuthDB");
    }
}
