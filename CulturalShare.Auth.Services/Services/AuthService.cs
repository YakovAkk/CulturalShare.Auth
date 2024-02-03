using AuthenticationProto;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Auth.Services.Configuration;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using Microsoft.Extensions.Logging;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace CulturalShare.Auth.Services.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;
    private readonly TokenConfiguration _tokenConfiguration;

    public AuthService(IPasswordService passwordService, IAuthRepository passwordRepository, ILogger<AuthService> logger, ITokenService tokenService, TokenConfiguration tokenConfiguration)
    {
        _passwordService = passwordService;
        _authRepository = passwordRepository;
        _logger = logger;
        _tokenService = tokenService;
        _tokenConfiguration = tokenConfiguration;
    }

    public async Task<int> CreateUserAsync(RegistrationRequest request)
    {
        _passwordService.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var user = new UserEntity()
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            LastName = request.LastName,
            FirstName = request.FirstName,
        };

        var result = await _authRepository.CreateUserAsync(user);
        return result;
    }

    public async Task<AccessKeyViewModel> GetAccessTokenAsync(LoginRequest request)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email);
        // TODO: Validate password

        if(user == null)
        {
            _logger.LogError($"{nameof(GetAccessTokenAsync)} request. User with email = {request.Email} doesn't exist!");
            throw new RowNotInTableException($"User with email = {request.Email} doesn't exist!");
        }

        return CreateAccessKey(user, DateTime.Now.AddDays(_tokenConfiguration.DaysUntilExpire), _tokenConfiguration.AuthorizationKey);
    }

    public AccessKeyViewModel GetOneTimeTokenAsync(GetOneTimeTokenRequest request)
    {
        var user = new UserEntity()
        {
            Id = request.UserId,
            Email = request.Email,
        };

        return CreateAccessKey(user, DateTime.Now, _tokenConfiguration.OneTimeAuthorizationKey);
    }

    #region Private
    private AccessKeyViewModel CreateAccessKey(UserEntity user, DateTime expiresAt, string authorizationKey)
    {
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
