using Serilog.Core;

namespace CulturalShare.Auth.API.Configuration.Base;

public interface IServiceInstaller
{
    void Install(WebApplicationBuilder builder, Logger logger);
}
