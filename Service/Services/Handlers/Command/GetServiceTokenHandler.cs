using AuthenticationProto;
using ErrorOr;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class GetServiceTokenHandler : IRequestHandler<GetServiceTokenCommand, ErrorOr<ServiceTokenResponse>>
{
    private readonly IAuthService _authService;
    public GetServiceTokenHandler(IAuthService authService) => _authService = authService;
    public Task<ErrorOr<ServiceTokenResponse>> Handle(GetServiceTokenCommand request, CancellationToken cancellationToken) =>
        _authService.GetServiceTokenAsync(request.Request);
}
