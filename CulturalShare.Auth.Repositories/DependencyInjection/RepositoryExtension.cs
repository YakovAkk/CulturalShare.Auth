﻿using CulturalShare.Auth.Repositories.Repositories;
using CulturalShare.Auth.Repositories.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CulturalShare.Auth.Repositories.DependencyInjection;

public static class RepositoryExtension
{
    public static IServiceCollection AddAuthRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();

        return services;
    }

}
