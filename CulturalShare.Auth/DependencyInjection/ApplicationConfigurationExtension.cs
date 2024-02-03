using CulturalShare.Auth.API.Configuration.Base;
using System.Reflection;

namespace CulturalShare.Auth.API.DependencyInjection;

public static class ApplicationConfigurationExtension
{
    public static WebApplicationBuilder InstallServices(this WebApplicationBuilder builder, params Assembly[] assemblies)
    {
        var serviceInstallers = assemblies.SelectMany(x => x.DefinedTypes)
              .Where(x => IsAssignableToType<IServiceInstaller>(x))
              .Select(Activator.CreateInstance)
              .Cast<IServiceInstaller>();

        foreach (var serviceInstaller in serviceInstallers)
        {
            serviceInstaller.Install(builder);
        }

        return builder;
    }

    private static bool IsAssignableToType<T>(TypeInfo type) =>
        typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;
}
