using AuthenticationProto;
using Grpc.Core;
using Service.Services.Base;

namespace WebApi.Services;

public class UserService : UserGrpcService.UserGrpcServiceBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserService userService, 
        ILogger<UserService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var id = await _userService.CreateUserAsync(request);

        return new CreateUserResponse()
        {
            Id = id
        };
    }
}
