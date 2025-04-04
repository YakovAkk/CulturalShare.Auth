using Microsoft.Extensions.DependencyInjection;
using Repositories.Infrastructure.Repositories;
using Repository.Repositories;

namespace Dependency.Infranstructure.DependencyInjection;

public static class RepositoryExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
        services.AddScoped<IFollowerEntityRepository, FollowerEntityRepository>();
        services.AddScoped<IRestrictedUserEntityRepository, RestrictedUserEntityRepository>();

        return services;
    }
}
