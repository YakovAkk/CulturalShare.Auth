using AuthenticationBackProto;
using AuthenticationProto;
using CulturalShare.Foundation.Authorization.JwtServices;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.Base;
using Service.Services.Handlers.Command;
using static Service.Handlers.MediatRCommands;

namespace Dependency.Infranstructure.DependencyInjection;

public static class ServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJwtBlacklistService, JwtBlacklistService>();

        services.AddScoped<IRequestHandler<RefreshTokenCommand, ErrorOr<RefreshTokenResponse>>, RefreshTokenHandler>();
        services.AddScoped<IRequestHandler<GetServiceTokenCommand, ErrorOr<ServiceTokenResponse>>, GetServiceTokenHandler>();
        services.AddScoped<IRequestHandler<GetUserTokenCommand, ErrorOr<UserTokenResponse>>, GetUserTokenCommandHandler>();
        services.AddScoped<IRequestHandler<RevokeUserTokenCommand, ErrorOr<Empty>>, RevokeUserTokenHandler>();

        return services;
    }
}
