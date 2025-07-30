using AuthenticationBackProto;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class RevokeUserTokenHandler : IRequestHandler<RevokeUserTokenCommand, ErrorOr<Empty>>
{
    private readonly IAuthService _authService;
    public RevokeUserTokenHandler(IAuthService authService) => _authService = authService;
    public Task<ErrorOr<Empty>> Handle(RevokeUserTokenCommand request, CancellationToken cancellationToken) =>
        _authService.RevokeUserTokenAsync(request.Request.UserId);
}
