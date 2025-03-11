using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Services.Model;
using System.IdentityModel.Tokens.Jwt;

namespace CulturalShare.Auth.Services.Services.Base;

public interface ITokenService
{
    public string CreateRandomToken();
    public JwtSecurityToken CreateAccessToken(UserEntity user, DateTime expiresAt, string authorizationKey);
    public RefreshToken CreateRefreshToken();
}
