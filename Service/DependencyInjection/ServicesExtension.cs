using CulturalShare.Auth.Services.Services;
using CulturalShare.Auth.Services.Services.Base;
using CulturalShare.Foundation.Authorization.JwtServices;
using Microsoft.Extensions.DependencyInjection;

namespace CulturalShare.Auth.Services.DependencyInjection;

public static class ServicesExtension
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJwtBlacklistService, JwtBlacklistService>();

        return services;
    }
}
