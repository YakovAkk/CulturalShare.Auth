using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Services.Model;
using Infrastructure.Configuration;
using Service.Model;

namespace CulturalShare.Auth.Services.Services.Base;

public interface ITokenService
{
    Task<AccessTokenViewModel> CreateAccessTokenForServiceAsync(JwtServiceCredentials jwtServiceCredentials);
    Task<AccessTokenViewModel> CreateAccessTokenForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserEntity user);
    Task<AccessAndRefreshTokenViewModel> CreateAccessAndRefreshTokensForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserEntity user);
}
