using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand, ErrorOr<Empty>>
{
    private readonly IUserService _userService;
    public UnfollowUserHandler(IUserService userService) => _userService = userService;
    public Task<ErrorOr<Empty>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken) =>
        _userService.UnfollowUserAsync(request.Request, request.UserId);
}
