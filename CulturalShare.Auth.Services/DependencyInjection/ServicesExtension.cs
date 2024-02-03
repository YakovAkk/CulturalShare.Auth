using CulturalShare.Auth.Services.Services;
using CulturalShare.Auth.Services.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CulturalShare.Auth.Services.DependencyInjection;

public static class ServicesExtension
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
