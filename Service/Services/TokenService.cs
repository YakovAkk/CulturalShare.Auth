using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Services.Configuration;
using CulturalShare.Auth.Services.Model;
using CulturalShare.Auth.Services.Services.Base;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CulturalShare.Auth.Services.Services;

public class TokenService : ITokenService
{
    private readonly JwtServicesConfig _jwtServicesSettings;

    public TokenService(JwtServicesConfig jwtServicesSettings)
    {
        _jwtServicesSettings = jwtServicesSettings;
    }

    public string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    public JwtSecurityToken CreateAccessToken(UserEntity user, DateTime expiresAt, string authorizationKey)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizationKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: _jwtServicesSettings.Issuer,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return token;
    }

    public RefreshToken CreateRefreshToken()
    {
        var refreshToken = new RefreshToken()
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            CreatedAt = DateTime.UtcNow
        };

        return refreshToken;
    }

    public JwtSecurityToken CreateAccessToken(JwtServiceCredentials jwtServiceCredentials, DateTime expiresAt, string authorizationKey)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, jwtServiceCredentials.ServiceId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizationKey));
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
