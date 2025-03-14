using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Services.Model;
using Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace CulturalShare.Auth.Services.Services.Base;

public interface ITokenService
{
    public string CreateRandomToken();
    public JwtSecurityToken CreateAccessToken(UserEntity user, DateTime expiresAt, JwtServiceCredentials jwtServiceCredentials);
    public JwtSecurityToken CreateAccessToken(JwtServiceCredentials jwtServiceCredentials, DateTime expiresAt);
    public RefreshToken CreateRefreshToken();
}
