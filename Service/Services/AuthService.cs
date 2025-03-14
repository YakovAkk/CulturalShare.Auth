using AuthenticationProto;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Auth.Services.Configuration;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using CultureShare.Foundation.Exceptions;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace CulturalShare.Auth.Services.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly TokenConfiguration _tokenConfiguration;
    private readonly JwtServicesConfig _jwtServicesSettings;
    private readonly IUserRepository _userRepository;

    public AuthService(
        IPasswordService passwordService,
        ILogger<AuthService> logger,
        ITokenService tokenService,
        TokenConfiguration tokenConfiguration,
        JwtServicesConfig jwtServicesSettings,
        IUserRepository userRepository)
    {
        _passwordService = passwordService;
        _logger = logger;
        _tokenService = tokenService;
        _tokenConfiguration = tokenConfiguration;
        _jwtServicesSettings = jwtServicesSettings;
        _userRepository = userRepository;
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogDebug($"{nameof(CreateUserAsync)} request. User = {request.FirstName} {request.LastName} registered");

        if (!string.IsNullOrEmpty(request.Password))
        {
            throw new BadRequestException("Password must not be empty!");
        }

        var email = request.Email.Trim();

        _passwordService.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var user = new UserEntity()
        {
            Email = email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,            
        };

        var result = _userRepository.Add(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogDebug($"Customer {user.Id} was created.");

        return user.Id;
    }

    public async Task<AccessKeyViewModel> GetSignInAsync(SignInRequest request)
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

        return CreateAccessKey(user, DateTime.UtcNow.AddDays(_tokenConfiguration.DaysUntilExpire), _tokenConfiguration.AuthorizationKey);
    }

    public ServiceTokenResponse GetServiceTokenAsync(ServiceTokenRequest request)
    {
        if (!_jwtServicesSettings.JwtServices.TryGetValue(request.ServiceId, out var serviceSecret))
        {
            throw new UnauthorizedAccessException();
        }

        if (request.ServiceSecret != serviceSecret)
        {
            throw new UnauthorizedAccessException();
        }

        var expiresAt = DateTime.UtcNow.AddSeconds(_jwtServicesSettings.SecondsUntilExpire);

        var jwtServiceCredentials = new JwtServiceCredentials
        {
            ServiceId = request.ServiceId,
            ServiceSecret = request.ServiceSecret
        };

        var token = CreateAccessKey(jwtServiceCredentials, expiresAt);

        var totalSeconds = (int)(token.ExpireDate - DateTime.UtcNow).TotalSeconds;

        return new ServiceTokenResponse
        {
            AccessToken = token.AccessToken,
            ExpiresInSeconds = totalSeconds
        };
    }

    #region Private
    private AccessKeyViewModel CreateAccessKey(JwtServiceCredentials jwtServiceCredentials, DateTime expiresAt)
    {
        var refreshToken = _tokenService.CreateRefreshToken();
        var accessToken = _tokenService.CreateAccessToken(jwtServiceCredentials, expiresAt);
        var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return new AccessKeyViewModel()
        {
            RefreshToken = refreshToken.Token,
            AccessToken = token,
            ExpireDate = accessToken.ValidTo,
        };
    }

    private AccessKeyViewModel CreateAccessKey(UserEntity user, DateTime expiresAt, string authorizationKey)
    {
        _logger.LogDebug($"{nameof(CreateAccessKey)} request. User Id = {user.Id}");

        var refreshToken = _tokenService.CreateRefreshToken();
        var accessToken = _tokenService.CreateAccessToken(user, expiresAt, authorizationKey);
        var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return new AccessKeyViewModel()
        {
            RefreshToken = refreshToken.Token,
            AccessToken = token,
            ExpireDate = accessToken.ValidTo,
        };
    }

    #endregion
}
