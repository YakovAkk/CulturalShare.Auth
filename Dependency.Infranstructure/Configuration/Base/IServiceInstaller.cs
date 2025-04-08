using Microsoft.AspNetCore.Builder;
using Serilog.Core;

namespace Dependency.Infranstructure.Configuration.Base;

public interface IServiceInstaller
{
    void Install(WebApplicationBuilder builder, Logger logger);
}
