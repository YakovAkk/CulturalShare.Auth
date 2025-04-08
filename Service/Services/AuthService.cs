using AuthenticationProto;
using CulturalShare.Foundation.Authorization.JwtServices;
using CulturalShare.Foundation.EntironmentHelper.Configurations;
using DomainEntity.Configuration;
using DomainEntity.Constants;
using DomainEntity.Entities;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
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
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly JwtServicesConfig _jwtServicesSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtBlacklistService _jwtBlacklistService;

    public AuthService(
        IPasswordService passwordService,
        ILogger<AuthService> logger,
        ITokenService tokenService,
        JwtServicesConfig jwtServicesSettings,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtBlacklistService jwtBlacklistService)
    {
        _passwordService = passwordService;
        _logger = logger;
        _tokenService = tokenService;
        _jwtServicesSettings = jwtServicesSettings;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtBlacklistService = jwtBlacklistService;
    }

    public async Task<ErrorOr<SignInResponse>> GetSignInAsync(SignInRequest request)
    {
        var user = await _userRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            _logger.LogError($"{nameof(GetSignInAsync)} request. User with email = {request.Email} doesn't exist!");
            return Error.NotFound("UserNotFound", $"User with email = {request.Email} doesn't exist!");
        }

        var isPasswordValid = _passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isPasswordValid)
        {
            _logger.LogDebug($"Invalid login attempt for user with email = {user.Email}");
            return Error.Validation("InvalidCredentials", "Email or password is incorrect.");
        }

        var jwtCredentialsResult = GetJwtCredentials(JwtTokenConstants.UserAudience);
        if (jwtCredentialsResult.IsError)
        {
            _logger.LogError("JWT credentials missing for service audience");
            return jwtCredentialsResult.Errors;
        }

        var jwtServiceCredentials = jwtCredentialsResult.Value;

        var accessTokenViewModel = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(jwtServiceCredentials, user);

        await _jwtBlacklistService.RemoveUserFromBlacklistAsync(user.Id);

        var signInResponse = accessTokenViewModel.ToSignInResponse();

        return signInResponse;
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

        var userResult = await GetUserByIdAsync(userId);
        if (userResult.IsError)
        {
            _logger.LogError("User not found for refresh token: {UserId}", userId);
            return userResult.Errors;
        }

        var user = userResult.Value;
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

    public async Task<ErrorOr<Empty>> SignOutAsync(int userId)
    {
        _logger.LogInformation("SignOut request received");

        var refreshTokens = await GetRefreshTokenForUserAsync(userId);
        await RevokeToken(refreshTokens);

        await _jwtBlacklistService.BlacklistUserAsync(userId, TimeSpan.MaxValue);

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
        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(audience, out var secret))
        {
            return Error.Unauthorized("JwtConfig.MissingSecret", "JWT secret for user audience is not configured.");
        }

        return new JwtServiceCredentials
        {
            ServiceId = JwtTokenConstants.UserAudience,
            ServiceSecret = secret
        };
    }

    private async Task<ErrorOr<UserEntity>> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            _logger.LogError($"{nameof(GetUserByIdAsync)} request. User with Id = {userId} doesn't exist!");
            return Error.NotFound("User.NotFound", $"User with Id = {userId} doesn't exist.");
        }

        return user;
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
