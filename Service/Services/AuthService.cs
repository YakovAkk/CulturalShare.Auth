using AuthenticationProto;
using CulturalShare.Foundation.EntironmentHelper.Configurations;
using DomainEntity.Configuration;
using DomainEntity.Constants;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Service.Mapping;
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

    public AuthService(
        IPasswordService passwordService,
        ILogger<AuthService> logger,
        ITokenService tokenService,
        JwtServicesConfig jwtServicesSettings,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _passwordService = passwordService;
        _logger = logger;
        _tokenService = tokenService;
        _jwtServicesSettings = jwtServicesSettings;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
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

        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(JwtTokenConstants.UserAudience, out var serviceSecret))
        {
            return Error.Unauthorized("InvalidTokenService", "JWT secret invalid.");
        }

        var jwtServiceCredentials = new JwtServiceCredentials
        {
            ServiceId = JwtTokenConstants.UserAudience,
            ServiceSecret = serviceSecret
        };

        var accessTokenViewModel = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(jwtServiceCredentials, user);

        var signInResponse = accessTokenViewModel.ToSignInResponse();

        return signInResponse;
    }


    public async Task<ErrorOr<ServiceTokenResponse>> GetServiceTokenAsync(ServiceTokenRequest request)
    {
        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(request.ServiceId, out var serviceSecret))
        {
            return Error.Unauthorized("ServiceNotRegistered", "The requested service ID is not registered.");
        }

        if (request.ServiceSecret != serviceSecret)
        {
            return Error.Unauthorized("InvalidSecret", "Invalid service credentials.");
        }

        var jwtServiceCredentials = new JwtServiceCredentials
        {
            ServiceId = request.ServiceId,
            ServiceSecret = request.ServiceSecret
        };

        var token = await _tokenService.CreateAccessTokenForServiceAsync(jwtServiceCredentials);

        var serviceTokenResponse = token.ToServiceTokenResponse();

        return serviceTokenResponse;
    }

    public async Task<ErrorOr<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, int userId)
    {
        var refreshToken = _refreshTokenRepository
            .GetAll()
            .FirstOrDefault(x => x.Token == request.RefreshToken && x.UserId == userId);

        if (refreshToken == null)
        {
            return Error.Unauthorized("RefreshToken.NotFound", "Refresh token does not exist.");
        }

        if (!refreshToken.IsActive)
        {
            return Error.Unauthorized("RefreshToken.Inactive", "Refresh token has expired.");
        }

        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(JwtTokenConstants.UserAudience, out var serviceSecret))
        {
            return Error.Unauthorized("JwtConfig.MissingSecret", "JWT secret for user audience is not configured.");
        }

        var jwtServiceCredentials = new JwtServiceCredentials
        {
            ServiceId = JwtTokenConstants.UserAudience,
            ServiceSecret = serviceSecret
        };

        var user = await _userRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            _logger.LogError($"{nameof(RefreshTokenAsync)} request. User with Id = {userId} doesn't exist!");
            return Error.NotFound("User.NotFound", $"User with Id = {userId} doesn't exist.");
        }

        // if refresh token is still valid for another access token
        if (refreshToken.ExpiresAt > DateTime.UtcNow.AddSeconds(_jwtServicesSettings.SecondsUntilExpireUserJwtToken))
        {
            var accessToken = await _tokenService.CreateAccessTokenForUserAsync(jwtServiceCredentials, user);

            return new RefreshTokenResponse
            {
                AccessToken = accessToken.AccessToken,
                AccessTokenExpiresInSeconds = (int)(accessToken.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresInSeconds = (int)(refreshToken.ExpiresAt - DateTime.UtcNow).TotalSeconds
            };
        }

        var accessRefreshTokenPair = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(jwtServiceCredentials, user);

        return new RefreshTokenResponse
        {
            AccessToken = accessRefreshTokenPair.AccessToken,
            AccessTokenExpiresInSeconds = (int)(accessRefreshTokenPair.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
            RefreshToken = accessRefreshTokenPair.RefreshToken,
            RefreshTokenExpiresInSeconds = (int)(accessRefreshTokenPair.RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds
        };
    }
}
