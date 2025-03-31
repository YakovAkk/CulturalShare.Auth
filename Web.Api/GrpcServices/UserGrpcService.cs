using AuthenticationProto;
using CulturalShare.Common.Helper.Extensions;
using Grpc.Core;
using Service.Services.Base;

namespace WebApi.GrpcServices;

public class UserGrpcService : AuthenticationProto.UserGrpcService.UserGrpcServiceBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(
        IUserService userService,
        ILogger<UserGrpcService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var result = await _userService.CreateUserAsync(request);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return new CreateUserResponse
        {
            Id = result.Value
        };
    }
}
