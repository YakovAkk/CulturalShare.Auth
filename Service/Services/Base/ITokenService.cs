using DomainEntity.Configuration;
using DomainEntity.Entities;
using Service.Model;

namespace Service.Services.Base;

public interface ITokenService
{
    Task<AccessTokenViewModel> CreateAccessTokenForServiceAsync(JwtServiceCredentials jwtServiceCredentials);
    Task<AccessTokenViewModel> CreateAccessTokenForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserEntity user);
    Task<AccessAndRefreshTokenViewModel> CreateAccessAndRefreshTokensForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserEntity user);
}
