using ErrorOr;
using MediatR;
using Service.Services.Base;
using static Service.Handlers.MediatRCommands;

namespace Service.Services.Handlers.Command;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, ErrorOr<int>>
{
    private readonly IUserService _userService;

    public CreateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<ErrorOr<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return _userService.CreateUserAsync(request.Request);
    }
}
