using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using CulturalShare.Foundation.EntironmentHelper.Configurations;
using DomainEntity.Entities;
using Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Repositories.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CulturalShare.Auth.Services.Services;

public class TokenService : ITokenService
{
    private readonly JwtServicesConfig _jwtServicesSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(
        JwtServicesConfig jwtServicesSettings, 
        IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtServicesSettings = jwtServicesSettings;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    public JwtSecurityToken CreateAccessToken(UserEntity user, DateTime expiresAt, JwtServiceCredentials jwtServiceCredentials)
    {
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

    public async Task<RefreshToken> CreateRefreshToken(int secondsUntilExpire)
    {
        var token = CreateRandomToken();

        var refreshToken = new RefreshTokenEntity(token, secondsUntilExpire);

        _refreshTokenRepository.Add(refreshToken);  
        await _refreshTokenRepository.SaveChangesAsync();

        return new RefreshToken()
        {
            Token = refreshToken.Token,
            IssuedAt = refreshToken.IssuedAt,
        };
    }

    public JwtSecurityToken CreateAccessToken(JwtServiceCredentials jwtServiceCredentials, DateTime expiresAt)
    {
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
}
