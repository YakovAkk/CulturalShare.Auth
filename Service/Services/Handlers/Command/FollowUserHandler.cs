using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class FollowUserHandler : IRequestHandler<FollowUserCommand, ErrorOr<Empty>>
{
    private readonly IUserService _userService;
    public FollowUserHandler(IUserService userService) => _userService = userService;
    public Task<ErrorOr<Empty>> Handle(FollowUserCommand request, CancellationToken cancellationToken) =>
        _userService.FollowUserAsync(request.Request, request.UserId);
}
