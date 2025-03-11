using AuthenticationProto;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Auth.Services.Configuration;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using CultureShare.Foundation.Exceptions;
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
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;
    private readonly TokenConfiguration _tokenConfiguration;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly UserManager<UserEntity> _userManager;

    public AuthService(
        IPasswordService passwordService,
        IAuthRepository passwordRepository,
        ILogger<AuthService> logger,
        ITokenService tokenService,
        TokenConfiguration tokenConfiguration,
        SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager)
    {
        _passwordService = passwordService;
        _authRepository = passwordRepository;
        _logger = logger;
        _tokenService = tokenService;
        _tokenConfiguration = tokenConfiguration;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<int> CreateUserAsync(CreateUserRequest request)
    {
        _logger.LogDebug($"{nameof(CreateUserAsync)} request. User = {request.FirstName} {request.LastName} registered");

        if (!string.IsNullOrEmpty(request.Password))
        {
            throw new BadRequestException("Password must not be empty!");
        }

        var email = request.Email.Trim();

        var user = new UserEntity()
        {
            Email = request.Email,
            LastName = request.LastName,
            FirstName = request.FirstName,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        await _signInManager.SignInAsync(user, false);

        _logger.LogDebug($"Customer {user.Id} was created.");

        return user.Id;
    }

    public async Task<AccessKeyViewModel> GetAccessTokenAsync(LoginRequest request)
    {
        if (!string.IsNullOrEmpty(request.Password))
        {
            throw new BadRequestException("Password must not be empty!");
        }

        SignInResult result = SignInResult.Success;
        var user = await _authRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            _logger.LogError($"{nameof(GetAccessTokenAsync)} request. User with email = {request.Email} doesn't exist!");
            throw new BadRequestException($"User with email = {request.Email} doesn't exist!");
        }

        result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (result.Succeeded)
        {
            _logger.LogDebug($"Customer {user.Id} was logged");

            return CreateAccessKey(user, DateTime.UtcNow.AddDays(_tokenConfiguration.DaysUntilExpire), _tokenConfiguration.AuthorizationKey);
        }

        throw new BadRequestException($"Email or/and password is incorrect");
    }

    public AccessKeyViewModel GetOneTimeTokenAsync(GetOneTimeTokenRequest request)
    {
        _logger.LogDebug($"{nameof(GetOneTimeTokenAsync)} request. {JsonConvert.SerializeObject(request)}");

        var user = new UserEntity()
        {
            Id = request.UserId,
            Email = request.Email,
        };

        return CreateAccessKey(user, DateTime.UtcNow, _tokenConfiguration.OneTimeAuthorizationKey);
    }

    #region Private
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
