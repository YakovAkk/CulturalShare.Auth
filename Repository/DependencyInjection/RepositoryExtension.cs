using CulturalShare.Auth.Repositories.Repositories;
using CulturalShare.Auth.Repositories.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;
using Repository.Repositories.Base;

namespace CulturalShare.Auth.Repositories.DependencyInjection;

public static class RepositoryExtension
{
    public static IServiceCollection AddAuthRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }

}
