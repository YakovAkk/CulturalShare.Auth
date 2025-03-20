using AuthenticationProto;
using Azure.Core;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using CulturalShare.Foundation.EntironmentHelper.Configurations;
using CultureShare.Foundation.Exceptions;
using Grpc.Core;
using Infrastructure.Configuration;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories.Base;
using Service.Model;

namespace CulturalShare.Auth.Services.Services;

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

    public async Task<AccessAndRefreshTokenViewModel> GetSignInAsync(SignInRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
        {
            throw new BadRequestException("Password must not be empty!");
        }

        var user = await _userRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            _logger.LogError($"{nameof(GetSignInAsync)} request. User with email = {request.Email} doesn't exist!");
            throw new BadRequestException($"User with email = {request.Email} doesn't exist!");
        }

        var isPasswordValid = _passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isPasswordValid)
        {
            _logger.LogDebug($"Customer {user.Id} was logged");

            throw new BadRequestException($"Email or/and password is incorrect");
        }

        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(JwtTokenConstants.UserAudience, out var serviceSecret))
        {
            throw new UnauthorizedAccessException();
        }

        var jwtServiceCredentials = new JwtServiceCredentials
        {
            ServiceId = JwtTokenConstants.UserAudience,
            ServiceSecret = serviceSecret
        };

        var accessTokenViewModel = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(jwtServiceCredentials, user);

        return accessTokenViewModel;
    }

    public async Task<ServiceTokenResponse> GetServiceTokenAsync(ServiceTokenRequest request)
    {
        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(request.ServiceId, out var serviceSecret))
        {
            throw new UnauthorizedAccessException();
        }

        if (request.ServiceSecret != serviceSecret)
        {
            throw new UnauthorizedAccessException();
        }

        var jwtServiceCredentials = new JwtServiceCredentials
        {
            ServiceId = request.ServiceId,
            ServiceSecret = request.ServiceSecret
        };

        var token = await _tokenService.CreateAccessTokenForServiceAsync(jwtServiceCredentials);
        var totalSeconds = (int)(token.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds;

        var serviceTokenResponse = new ServiceTokenResponse
        {
            AccessToken = token.AccessToken,
            ExpiresInSeconds = totalSeconds
        };

        return serviceTokenResponse;
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, int userId)
    {
        var refreshToken = _refreshTokenRepository
            .GetAll()
            .FirstOrDefault(x => x.Token == request.RefreshToken && x.UserId == userId);

        if (refreshToken == null)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Token does not exist."));
        }

        if (!refreshToken.IsActive)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Token has expired."));
        }

        if (!_jwtServicesSettings.JwtSecretTokenPairs.TryGetValue(JwtTokenConstants.UserAudience, out var serviceSecret))
        {
            throw new UnauthorizedAccessException();
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
            _logger.LogError($"{nameof(GetSignInAsync)} request. User with Id = {userId} doesn't exist!");
            throw new BadRequestException($"User with Id = {userId} doesn't exist!");
        }

        // if refresh token can survive an other access token 
        if (refreshToken.ExpiresAt > DateTime.UtcNow.AddSeconds(_jwtServicesSettings.SecondsUntilExpireUserJwtToken))
        {
            var accessToken = await _tokenService.CreateAccessTokenForUserAsync(jwtServiceCredentials, user);

            return new RefreshTokenResponse
            {
                AccessToken = accessToken.AccessToken,
                AccessTokenExpiresInSeconds = (int)(accessToken.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
                RefreshTokenExpiresInSeconds = (int)(refreshToken.ExpiresAt - DateTime.UtcNow).TotalSeconds,
                RefreshToken = refreshToken.Token
            };
        }

        var accessRefreshTokenPair = await _tokenService.CreateAccessAndRefreshTokensForUserAsync(jwtServiceCredentials, user);

        return new RefreshTokenResponse
        {
            AccessToken = accessRefreshTokenPair.AccessToken,
            AccessTokenExpiresInSeconds = (int)(accessRefreshTokenPair.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
            RefreshTokenExpiresInSeconds = (int)(accessRefreshTokenPair.RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds,
            RefreshToken = accessRefreshTokenPair.RefreshToken
        };
    }

    #region Private

    #endregion
}
