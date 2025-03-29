using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Serilog.Core;
using System.Reflection;

namespace Dependency.Infranstructure.DependencyInjection;

public static class ApplicationConfigurationExtension
{
    public static WebApplicationBuilder InstallServices(this WebApplicationBuilder builder, Logger logger, params Assembly[] assemblies)
    {
        var serviceInstallers = assemblies.SelectMany(x => x.DefinedTypes)
              .Where(x => IsAssignableToType<IServiceInstaller>(x))
              .Select(Activator.CreateInstance)
              .Cast<IServiceInstaller>();

        foreach (var serviceInstaller in serviceInstallers)
        {
            serviceInstaller.Install(builder, logger);
        }

        return builder;
    }

    private static bool IsAssignableToType<T>(TypeInfo type) =>
        typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;
}
