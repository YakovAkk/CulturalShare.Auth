using AuthenticationProto;
using CulturalShare.Foundation.Authorization.JwtServices;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.Base;
using Service.Services.Handlers.Command;
using Service.Services.Handlers.Queries;
using static Service.Handlers.MediatRCommands;
using static Service.Handlers.MediatRQueries;

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

        services.AddScoped<IRequestHandler<SignInCommand, ErrorOr<SignInResponse>>, SignInHandler>();
        services.AddScoped<IRequestHandler<RefreshTokenCommand, ErrorOr<RefreshTokenResponse>>, RefreshTokenHandler>();
        services.AddScoped<IRequestHandler<GetServiceTokenCommand, ErrorOr<ServiceTokenResponse>>, GetServiceTokenHandler>();
        services.AddScoped<IRequestHandler<SignOutCommand, ErrorOr<Empty>>, SignOutHandler>();

        services.AddScoped<IRequestHandler<CreateUserCommand, ErrorOr<int>>, CreateUserHandler>();
        services.AddScoped<IRequestHandler<FollowUserCommand, ErrorOr<Empty>>, FollowUserHandler>();
        services.AddScoped<IRequestHandler<UnfollowUserCommand, ErrorOr<Empty>>, UnfollowUserHandler>();
        services.AddScoped<IRequestHandler<AllowUserCommand, ErrorOr<Empty>>, AllowUserHandler>();
        services.AddScoped<IRequestHandler<RestrictUserCommand, ErrorOr<Empty>>, RestrictUserHandler>();
        services.AddScoped<IRequestHandler<ToggleNotificationsCommand, ErrorOr<Empty>>, ToggleNotificationsHandler>();
        services.AddScoped<IRequestHandler<SearchUserByNameQuery, ErrorOr<SearchUserResponse>>, SearchUserByNameHandler>();

        return services;
    }
}
