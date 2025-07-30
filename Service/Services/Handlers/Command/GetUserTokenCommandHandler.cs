using AuthenticationBackProto;
using ErrorOr;
using MediatR;
using Service.Handlers;
using Service.Services.Base;

namespace Service.Services.Handlers.Command;

public class GetUserTokenCommandHandler : IRequestHandler<MediatRCommands.GetUserTokenCommand, ErrorOr<UserTokenResponse>>
{
    private readonly IAuthService _authService;

    public GetUserTokenCommandHandler(IAuthService authService) => _authService = authService;

    public Task<ErrorOr<UserTokenResponse>> Handle(MediatRCommands.GetUserTokenCommand request, CancellationToken cancellationToken) =>
        _authService.GenerateUserTokenAsync(request.Request);
}