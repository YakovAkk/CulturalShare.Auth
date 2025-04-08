using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class RestrictUserHandler : IRequestHandler<RestrictUserCommand, ErrorOr<Empty>>
{
    private readonly IUserService _userService;
    public RestrictUserHandler(IUserService userService) => _userService = userService;
    public Task<ErrorOr<Empty>> Handle(RestrictUserCommand request, CancellationToken cancellationToken) =>
        _userService.RestrictUserAsync(request.Request, request.UserId);
}
