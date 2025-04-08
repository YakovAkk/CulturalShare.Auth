using AuthenticationProto;
using ErrorOr;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<RefreshTokenResponse>>
{
    private readonly IAuthService _authService;
    public RefreshTokenHandler(IAuthService authService) => _authService = authService;
    public Task<ErrorOr<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        _authService.RefreshTokenAsync(request.Request, request.UserId);
}
