using CulturalShare.Auth.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CulturalShare.Auth.Services.DependencyInjection;

public static class ConfigurationExtension
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenConfig = configuration
            .GetSection("JwtSettings")
            .Get<TokenConfiguration>();
        services.AddSingleton(tokenConfig);

        return services;
    }
}

