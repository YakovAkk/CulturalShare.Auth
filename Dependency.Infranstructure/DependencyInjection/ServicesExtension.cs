using CulturalShare.Foundation.Authorization.JwtServices;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.Base;

namespace Dependency.Infranstructure.DependencyInjection;

public static class ServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJwtBlacklistService, JwtBlacklistService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
