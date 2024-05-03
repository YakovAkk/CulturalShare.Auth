using AuthenticationProto;
using CulturalShare.Auth.Services.Services.Base;
using Grpc.Core;
using Newtonsoft.Json;

namespace CulturalShare.Auth.Services;

public class AuthenticationService : Authentication.AuthenticationBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationService> _log;
    public AuthenticationService(IAuthService authService, ILogger<AuthenticationService> log)
    {
        _authService = authService;
        _log = log;
    }

    public override async Task<RegistrationReply> Registration(RegistrationRequest request, ServerCallContext context)
    {
        _log.LogDebug($"{nameof(Registration)} request. Body = {JsonConvert.SerializeObject(request)}");

        var id = await _authService.CreateUserAsync(request);

        return new RegistrationReply() 
        { 
            Id = id,
        };
    }

    public override async Task<AccessTokenReply> Login(LoginRequest request, ServerCallContext context)
    {
        _log.LogDebug($"{nameof(Login)} request. Body = {JsonConvert.SerializeObject(request)}");

        var accessToken = await _authService.GetAccessTokenAsync(request);

        TimeSpan remainingTime = accessToken.ExpireDate - DateTime.Now;
        int remainingSeconds = (int)remainingTime.TotalSeconds;

        return new AccessTokenReply()
        {
            AccessToken = accessToken.AccessToken,
            ExpiresInSeconds = remainingSeconds
        };
    }

    public override async Task<AccessTokenReply> GetOneTimeToken(GetOneTimeTokenRequest request, ServerCallContext context)
    {
        _log.LogDebug($"{nameof(GetOneTimeToken)} request. Body = {JsonConvert.SerializeObject(request)}");

        var accessToken = _authService.GetOneTimeTokenAsync(request);

        TimeSpan remainingTime = accessToken.ExpireDate - DateTime.Now;
        int remainingSeconds = (int)remainingTime.TotalSeconds;

        return new AccessTokenReply()
        {
            AccessToken = accessToken.AccessToken,
            ExpiresInSeconds = remainingSeconds
        };
    }

    public override Task<AccessTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        _log.LogDebug($"{nameof(RefreshToken)} request. Body = {JsonConvert.SerializeObject(request)}");

        throw new RpcException(new Status(StatusCode.Unavailable, "Not implemented"));
    }
}
