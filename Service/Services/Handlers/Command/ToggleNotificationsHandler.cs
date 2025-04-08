using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class ToggleNotificationsHandler : IRequestHandler<ToggleNotificationsCommand, ErrorOr<Empty>>
{
    private readonly IUserService _userService;
    public ToggleNotificationsHandler(IUserService userService) => _userService = userService;
    public Task<ErrorOr<Empty>> Handle(ToggleNotificationsCommand request, CancellationToken cancellationToken) =>
        _userService.ToggleNotificationsAsync(request.Request, request.UserId);
}
