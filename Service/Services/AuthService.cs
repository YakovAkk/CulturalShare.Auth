using AuthenticationBackProto;
using AuthenticationProto;
using CulturalShare.Foundation.Authorization.JwtServices;
using CulturalShare.Foundation.EnvironmentHelper.Configurations;
using DomainEntity.Configuration;
using DomainEntity.Constants;
using DomainEntity.Entities;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Service.Mapping;
using Service.Model;
using Service.Services.Base;

namespace Service.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly ITokenService _tokenService;
    private readonly JwtServicesConfig _jwtServicesSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtBlacklistService _jwtBlacklistService;

    public AuthService(
        ILogger<AuthService> logger,
        ITokenService tokenService,
        JwtServicesConfig jwtServicesSettings,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtBlacklistService jwtBlacklistService)
    {
        _logger = logger;
        _tokenService = tokenService;
        _jwtServicesSettings = jwtServicesSettings;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtBlacklistService = jwtBlacklistService;
    }

    public async Task<ErrorOr<UserTokenResponse>> GenerateUserTokenAsync(UserTokenRequest request)
    {
        var jwtCredentialsResult = GetJwtCredentials(JwtTokenConstants.UserAudience);

        if (jwtCredentialsResult.IsError)
        {
            _logger.LogError("JWT credentials missing for user audience");
            return jwtCredentialsResult.Errors;
        }

        var credentials = jwtCredentialsResult.Value;

        var accessRefreshTokenPair = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(credentials, request);

        await _jwtBlacklistService.RemoveUserFromBlacklistAsync(request.UserId.ToString());

        return new UserTokenResponse()
        {
            AccessToken = accessRefreshTokenPair.AccessToken,
            AccessTokenExpiresInSeconds = (int)(accessRefreshTokenPair.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
            RefreshToken = accessRefreshTokenPair.RefreshToken,
            RefreshTokenExpiresInSeconds = (int)(accessRefreshTokenPair.RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds
        };
    }

    public async Task<ErrorOr<ServiceTokenResponse>> GetServiceTokenAsync(ServiceTokenRequest request)
    {
        var jwtCredentialsResult = GetJwtCredentials(request.ServiceId);
        if (jwtCredentialsResult.IsError)
        {
            _logger.LogError("JWT credentials missing for service audience");
            return jwtCredentialsResult.Errors;
        }

        var jwtServiceCredentials = jwtCredentialsResult.Value;

        if (request.ServiceSecret != jwtServiceCredentials.ServiceSecret)
        {
            return Error.Unauthorized("InvalidSecret", "Invalid service credentials.");
        }

        var token = await _tokenService.CreateAccessTokenForServiceAsync(jwtServiceCredentials);

        var serviceTokenResponse = token.ToServiceTokenResponse();

        return serviceTokenResponse;
    }

    public async Task<ErrorOr<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, int userId)
    {
        _logger.LogInformation("RefreshTokenAsync request received for userId: {UserId}", userId);

        var refreshToken = await GetValidRefreshTokenAsync(request.RefreshToken, userId);
        if (refreshToken.IsError)
        {
            _logger.LogWarning("Refresh token validation failed: {Error}", refreshToken.FirstError.Description);
            return refreshToken.Errors;
        }

        var jwtCredentialsResult = GetJwtCredentials(JwtTokenConstants.UserAudience);
        if (jwtCredentialsResult.IsError)
        {
            _logger.LogError("JWT credentials missing for user audience");
            return jwtCredentialsResult.Errors;
        }

        var user = request.User;
        var credentials = jwtCredentialsResult.Value;

        // if refresh token is still valid for another access token
        if (IsRefreshTokenStillFresh(refreshToken.Value))
        {
            var accessToken = await _tokenService.CreateAccessTokenForUserAsync(credentials, user);
            return GetAccessTokenWithExistingRefresh(refreshToken.Value, accessToken);
        }

        var accessRefreshTokenPair = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(credentials, user);
        return GetAccessTokenWithNewRefresh(accessRefreshTokenPair);
    }

    public async Task<ErrorOr<Empty>> RevokeUserTokenAsync(int userId)
    {
        _logger.LogInformation("SignOut request received");

        var refreshTokens = await GetRefreshTokenForUserAsync(userId);
        await RevokeToken(refreshTokens);

        await _jwtBlacklistService.BlacklistUserAsync(userId.ToString(), TimeSpan.MaxValue);

        _logger.LogInformation("User with id {UserId} successfully signed out", userId);
        return new Empty();
    }

    #region Private

    private static ErrorOr<RefreshTokenResponse> GetAccessTokenWithNewRefresh(AccessAndRefreshTokenViewModel accessRefreshTokenPair)
    {
        return new RefreshTokenResponse
        {
            AccessToken = accessRefreshTokenPair.AccessToken,
            AccessTokenExpiresInSeconds = (int)(accessRefreshTokenPair.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
            RefreshToken = accessRefreshTokenPair.RefreshToken,
            RefreshTokenExpiresInSeconds = (int)(accessRefreshTokenPair.RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds
        };
    }

    private static ErrorOr<RefreshTokenResponse> GetAccessTokenWithExistingRefresh(RefreshTokenEntity refreshToken, AccessTokenViewModel accessToken)
    {
        return new RefreshTokenResponse
        {
            AccessToken = accessToken.AccessToken,
            AccessTokenExpiresInSeconds = (int)(accessToken.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresInSeconds = (int)(refreshToken.ExpiresAt - DateTime.UtcNow).TotalSeconds
        };
    }

    private async Task<ErrorOr<RefreshTokenEntity>> GetValidRefreshTokenAsync(string token, int userId)
    {
        var refreshToken = await _refreshTokenRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Token == token && x.UserId == userId);

        if (refreshToken == null)
        {
            return Error.Unauthorized("RefreshToken.NotFound", "Refresh token does not exist.");
        }

        if (!refreshToken.IsActive)
        {
            return Error.Unauthorized("RefreshToken.Inactive", "Refresh token has expired.");
        }

        return refreshToken;
    }

    private ErrorOr<JwtServiceCredentials> GetJwtCredentials(string audience)
    {
        var serviceConfig = _jwtServicesSettings.ServicesJwtConfigs
            .FirstOrDefault(x => x.ServiceId == audience);

        if (serviceConfig == null || string.IsNullOrWhiteSpace(serviceConfig.ServiceSecret))
        {
            return Error.Unauthorized("JwtConfig.MissingSecret", "JWT secret for user audience is not configured.");
        }

        return new JwtServiceCredentials
        {
            ServiceId = audience,
            ServiceSecret = serviceConfig.ServiceSecret
        };
    }

    private bool IsRefreshTokenStillFresh(RefreshTokenEntity refreshToken)
    {
        return refreshToken.ExpiresAt > DateTime.UtcNow.AddSeconds(_jwtServicesSettings.SecondsUntilExpireUserJwtToken);
    }

    private async Task<List<RefreshTokenEntity>> GetRefreshTokenForUserAsync(int userId)
    {
        var dtTimeNow = DateTime.UtcNow;

        var refreshTokens = await _refreshTokenRepository
            .GetAll()
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiresAt > dtTimeNow)
            .ToListAsync();

        return refreshTokens;
    }

    private Task RevokeToken(IEnumerable<RefreshTokenEntity> refreshTokens)
    {
        Array.ForEach(refreshTokens.ToArray(), x => x.Revoke());
        _refreshTokenRepository.UpdateRange(refreshTokens);

        return _refreshTokenRepository.SaveChangesAsync();
    }

    #endregion
}
