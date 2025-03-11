using AuthenticationProto;
using CulturalShare.Auth.Services.Services.Base;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace CulturalShare.Auth.Services;

public class AuthenticationService : Authentication.AuthenticationBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationService> _logger;
    public AuthenticationService(IAuthService authService, ILogger<AuthenticationService> log)
    {
        _authService = authService;
        _logger = log;
    }

    public override async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request, ServerCallContext context)
    {
        var id = await _authService.CreateUserAsync(request);

        return new CreateUserResponse() 
        { 
            Id = id,
        };
    }

    public override async Task<AccessTokenResponse> LoginAsync(LoginRequest request, ServerCallContext context)
    {
        var accessToken = await _authService.GetAccessTokenAsync(request);

        TimeSpan remainingTime = accessToken.ExpireDate - DateTime.UtcNow;
        int remainingSeconds = (int)remainingTime.TotalSeconds;

        return new AccessTokenResponse()
        {
            AccessToken = accessToken.AccessToken,
            ExpiresInSeconds = remainingSeconds
        };
    }

    [Authorize]
    public override async Task<AccessTokenResponse> GetOneTimeTokenAsync(GetOneTimeTokenRequest request, ServerCallContext context)
    {
        var accessToken = _authService.GetOneTimeTokenAsync(request);

        TimeSpan remainingTime = accessToken.ExpireDate - DateTime.UtcNow;
        int remainingSeconds = (int)remainingTime.TotalSeconds;

        return new AccessTokenResponse()
        {
            AccessToken = accessToken.AccessToken,
            ExpiresInSeconds = remainingSeconds
        };
    }

    public override Task<AccessTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Unavailable, "Not implemented"));
    }
}
