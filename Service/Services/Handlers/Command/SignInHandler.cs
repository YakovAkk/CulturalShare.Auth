using AuthenticationProto;
using ErrorOr;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class SignInHandler : IRequestHandler<SignInCommand, ErrorOr<SignInResponse>>
{
    private readonly IAuthService _authService;
    public SignInHandler(IAuthService authService) => _authService = authService;
    public Task<ErrorOr<SignInResponse>> Handle(SignInCommand request, CancellationToken cancellationToken) =>
        _authService.GetSignInAsync(request.Request);
}
