using AuthenticationProto;
using CulturalShare.Common.Helper.Extensions;
using CulturalShare.Foundation.AspNetCore.Extensions.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Service.Services.Base;

namespace WebApi.GrpcServices;

public class UserGrpcService : AuthenticationProto.UserGrpcService.UserGrpcServiceBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserGrpcService(
        IUserService userService,
        ILogger<UserGrpcService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
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

    [Authorize]
    public override async Task<Empty> AllowUser(AllowUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(_httpContextAccessor.HttpContext);

        var result = await _userService.AllowUserAsync(request, userId);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> FollowUser(FollowUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(_httpContextAccessor.HttpContext);

        var result = await _userService.FollowUserAsync(request, userId);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> UnfollowUser(UnfollowUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(_httpContextAccessor.HttpContext);

        var result = await _userService.UnfollowUserAsync(request, userId);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> RestrictUser(RestrictUserRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(_httpContextAccessor.HttpContext);

        var result = await _userService.RestrictUserAsync(request, userId);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<SearchUserResponse> SearchUserByName(SearchUserRequest request, ServerCallContext context)
    {
        var result = await _userService.SearchUserByNameAsync(request);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<Empty> ToggleNotifications(ToggleNotificationsRequest request, ServerCallContext context)
    {
        var userId = HttpHelper.GetUserIdOrThrowRpcException(_httpContextAccessor.HttpContext);

        var result = await _userService.ToggleNotificationsAsync(request, userId);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }
}
