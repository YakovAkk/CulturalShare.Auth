using AuthenticationProto;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Service.Handlers;

public class MediatRCommands
{
    public record SignInCommand(SignInRequest Request) : IRequest<ErrorOr<SignInResponse>>;
    public record RefreshTokenCommand(RefreshTokenRequest Request, int UserId) : IRequest<ErrorOr<RefreshTokenResponse>>;
    public record GetServiceTokenCommand(ServiceTokenRequest Request) : IRequest<ErrorOr<ServiceTokenResponse>>;
    public record SignOutCommand(int UserId) : IRequest<ErrorOr<Empty>>;

    public record CreateUserCommand(CreateUserRequest Request) : IRequest<ErrorOr<int>>;
    public record FollowUserCommand(FollowUserRequest Request, int UserId) : IRequest<ErrorOr<Empty>>;
    public record UnfollowUserCommand(UnfollowUserRequest Request, int UserId) : IRequest<ErrorOr<Empty>>;
    public record AllowUserCommand(AllowUserRequest Request, int UserId) : IRequest<ErrorOr<Empty>>;
    public record RestrictUserCommand(RestrictUserRequest Request, int UserId) : IRequest<ErrorOr<Empty>>;
    public record ToggleNotificationsCommand(ToggleNotificationsRequest Request, int UserId) : IRequest<ErrorOr<Empty>>;
}
