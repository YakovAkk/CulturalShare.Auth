using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CulturalShare.Auth.Services.DependencyInjection;

public static class ConfigurationExtension
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}

