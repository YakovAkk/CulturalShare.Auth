using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class SignOutHandler : IRequestHandler<SignOutCommand, ErrorOr<Empty>>
{
    private readonly IAuthService _authService;
    public SignOutHandler(IAuthService authService) => _authService = authService;
    public Task<ErrorOr<Empty>> Handle(SignOutCommand request, CancellationToken cancellationToken) =>
        _authService.SignOutAsync(request.UserId);
}
