using AuthenticationProto;
using CulturalShare.Auth.Services.Services.Base;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CulturalShare.Auth.Services;

public class AuthenticationService : AuthenticationGrpcService.AuthenticationGrpcServiceBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IAuthService authService, ILogger<AuthenticationService> log)
    {
        _authService = authService;
        _logger = log;
    }

    public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(SignIn)} request. Email = {request.Email}");

        var accessToken = await _authService.GetSignInAsync(request);

        var accessTokenRemainingTime = accessToken.AccessTokenExpiresAt - DateTime.UtcNow;
        var refreshTokenRemainingTime = accessToken.RefreshTokenExpiresAt - DateTime.UtcNow;

        return new SignInResponse
        {
            AccessToken = accessToken.AccessToken,
            AccessTokenExpiresInSeconds = (int)accessTokenRemainingTime.TotalSeconds,
            RefreshToken = accessToken.RefreshToken,
            RefreshTokenExpiresInSeconds = (int)refreshTokenRemainingTime.TotalSeconds
        };
    }

    public override async Task<ServiceTokenResponse> GetServiceToken(ServiceTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(GetServiceToken)} request. ServiceName = {request.ServiceId}");

        var response = await _authService.GetServiceTokenAsync(request);

        return response;
    }

    [Authorize]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        // Access HttpContext from ServerCallContext
        var httpContext = context.GetHttpContext();

        // Retrieve the user claims from the JWT token
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User ID claim not found."));
        }

        var userId = Convert.ToInt32(userIdClaim.Value);

        _logger.LogDebug($"{nameof(RefreshToken)} request from UserId: {userId}");

        var response = await _authService.RefreshTokenAsync(request, userId);

        return response;
    }
}
