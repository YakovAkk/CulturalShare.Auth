using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using CulturalShare.Foundation.EntironmentHelper.Configurations;
using DomainEntity.Entities;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Repository.Repositories.Base;
using Service.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CulturalShare.Auth.Services.Services;

public class TokenService : ITokenService
{
    private readonly JwtServicesConfig _jwtServicesSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        JwtServicesConfig jwtServicesSettings,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<TokenService> logger)
    {
        _jwtServicesSettings = jwtServicesSettings;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public Task<AccessTokenViewModel> CreateAccessTokenForServiceAsync(JwtServiceCredentials jwtServiceCredentials)
    {
        _logger.LogDebug($"{nameof(CreateAccessTokenForServiceAsync)} request. Service Id = {jwtServiceCredentials.ServiceId}");

        var accessToken = CreateAccessTokenForServiceInternal(jwtServiceCredentials);
        var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

        var accessTokenViewModel = new AccessTokenViewModel()
        {
            AccessToken = token,
            AccessTokenExpiresAt = accessToken.ValidTo,
        };

        return Task.FromResult(accessTokenViewModel);
    }

    public Task<AccessTokenViewModel> CreateAccessTokenForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserEntity user)
    {
        _logger.LogDebug($"{nameof(CreateAccessTokenForUserAsync)} request. User Id = {user.Id}");

        var accessToken = CreateAccessTokenForUserInternal(jwtServiceCredentials, user);
        var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

        var accessTokenViewModel = new AccessTokenViewModel()
        {
            AccessToken = token,
            AccessTokenExpiresAt = accessToken.ValidTo,
        };

        return Task.FromResult(accessTokenViewModel);
    }

    public async Task<AccessAndRefreshTokenViewModel> CreateAccessAndRefreshTokensForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserEntity user)
    {
        _logger.LogDebug($"{nameof(CreateAccessAndRefreshTokensForUserAsync)} request. User Id = {user.Id}");

        var refreshTokenTask = CreateRefreshToken(_jwtServicesSettings.SecondsUntilExpireUserRefreshToken, user.Id);
        var accessTokenTask = CreateAccessTokenForUserAsync(jwtServiceCredentials, user);

        await Task.WhenAll(refreshTokenTask, accessTokenTask);

        var refreshToken = refreshTokenTask.Result;
        var accessToken = accessTokenTask.Result;

        return new AccessAndRefreshTokenViewModel()
        {
            RefreshToken = refreshToken.Token,
            AccessToken = accessToken.AccessToken,
            AccessTokenExpiresAt = accessToken.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }

    #region Private

    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    private async Task<RefreshToken> CreateRefreshToken(int secondsUntilExpire, int userId)
    {
        var token = CreateRandomToken();

        var refreshToken = new RefreshTokenEntity(token, secondsUntilExpire, userId);

        _refreshTokenRepository.Add(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new RefreshToken()
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
        };
    }

    private JwtSecurityToken CreateAccessTokenForUserInternal(JwtServiceCredentials jwtServiceCredentials, UserEntity user)
    {
        var expiresAt = DateTime.UtcNow.AddSeconds(_jwtServicesSettings.SecondsUntilExpireServiceJwtToken);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtServiceCredentials.ServiceSecret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: _jwtServicesSettings.Issuer,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds,
            audience: jwtServiceCredentials.ServiceId);

        return token;
    }

    private JwtSecurityToken CreateAccessTokenForServiceInternal(JwtServiceCredentials jwtServiceCredentials)
    {
        var expiresAt = DateTime.UtcNow.AddSeconds(_jwtServicesSettings.SecondsUntilExpireServiceJwtToken);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, jwtServiceCredentials.ServiceId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtServiceCredentials.ServiceSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: _jwtServicesSettings.Issuer,
            audience: jwtServiceCredentials.ServiceId,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return token;
    }

    #endregion
}
