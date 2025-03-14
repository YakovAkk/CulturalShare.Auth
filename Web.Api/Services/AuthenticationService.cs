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

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var id = await _authService.CreateUserAsync(request);

        return new CreateUserResponse() 
        { 
            Id = id
        };
    }

    public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        var accessToken = await _authService.GetSignInAsync(request);

        TimeSpan remainingTime = accessToken.ExpireDate - DateTime.UtcNow;  
        int remainingSeconds = (int)remainingTime.TotalSeconds;

        return new SignInResponse()
        {
            AccessToken = accessToken.AccessToken,
            ExpiresInSeconds = remainingSeconds,
            RefreshToken = accessToken.RefreshToken
        };
    }

    public override async Task<ServiceTokenResponse> GetServiceToken(ServiceTokenRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"{nameof(GetServiceToken)} request. ServiceName = {request.ServiceId}");

        var response = _authService.GetServiceTokenAsync(request);

        return response;
    }

    [Authorize]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        return new RefreshTokenResponse()
        {

        };
    }
}
