using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class AllowUserHandler : IRequestHandler<AllowUserCommand, ErrorOr<Empty>>
{
    private readonly IUserService _userService;
    public AllowUserHandler(IUserService userService) => _userService = userService;
    public Task<ErrorOr<Empty>> Handle(AllowUserCommand request, CancellationToken cancellationToken) =>
        _userService.AllowUserAsync(request.Request, request.UserId);
}
