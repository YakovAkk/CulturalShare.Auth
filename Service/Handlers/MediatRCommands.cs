using AuthenticationBackProto;
using AuthenticationProto;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;

namespace Service.Handlers;

public class MediatRCommands
{
    public record RefreshTokenCommand(RefreshTokenRequest Request, int UserId) : IRequest<ErrorOr<RefreshTokenResponse>>;
    public record GetServiceTokenCommand(ServiceTokenRequest Request) : IRequest<ErrorOr<ServiceTokenResponse>>;
    public record GetUserTokenCommand(UserTokenRequest Request) : IRequest<ErrorOr<UserTokenResponse>>;
    public record RevokeUserTokenCommand(RevokeUserTokenRequest Request) : IRequest<ErrorOr<Empty>>;
}
