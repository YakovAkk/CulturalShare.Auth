using AuthenticationProto;
using CulturalShare.Common.Helper.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Service.Services.Base;
using System.Security.Claims;

namespace WebApi.GrpcServices;

public class AuthenticationGrpcService : AuthenticationProto.AuthenticationGrpcService.AuthenticationGrpcServiceBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationGrpcService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationGrpcService(
        IAuthService authService,
        ILogger<AuthenticationGrpcService> log,
        IHttpContextAccessor httpContextAccessor)
    {
        _authService = authService;
        _logger = log;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(SignIn)} request. Email = {request.Email}");

        var accessTokenResult = await _authService.GetSignInAsync(request);

        accessTokenResult.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return accessTokenResult.Value;
    }

    [Authorize]
    public override async Task<Empty> SignOut(SignOutRequest request, ServerCallContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var accessTokenResult = await _authService.SignOutAsync(httpContext);

        accessTokenResult.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return accessTokenResult.Value;
    }

    public override async Task<ServiceTokenResponse> GetServiceToken(ServiceTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(GetServiceToken)} request. ServiceId = {request.ServiceId}");

        var result = await _authService.GetServiceTokenAsync(request);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }

    [Authorize]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User ID claim not found."));
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid User ID claim format."));
        }

        _logger.LogDebug($"{nameof(RefreshToken)} request from UserId: {userId}");

        var result = await _authService.RefreshTokenAsync(request, userId);

        result.ThrowRpcExceptionBasedOnErrorIfNeeded();

        return result.Value;
    }
}
