using AuthenticationBackProto;
using DomainEntity.Configuration;
using DomainEntity.Entities;
using Service.Model;

namespace Service.Services.Base;

public interface ITokenService
{
    Task<AccessTokenViewModel> CreateAccessTokenForServiceAsync(JwtServiceCredentials jwtServiceCredentials);
    Task<AccessTokenViewModel> CreateAccessTokenForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserTokenRequest user);
    Task<AccessAndRefreshTokenViewModel> CreateAccessAndRefreshTokensForUserAsync(JwtServiceCredentials jwtServiceCredentials, UserTokenRequest user);
}
