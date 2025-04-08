using AuthenticationProto;
using Riok.Mapperly.Abstractions;
using Service.Model;

namespace Service.Mapping;

[Mapper]
public static partial class CommonMapper
{
    public static ServiceTokenResponse ToServiceTokenResponse(this AccessTokenViewModel token)
    {
        var totalSeconds = (int)(token.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds;

        var serviceTokenResponse = new ServiceTokenResponse
        {
            AccessToken = token.AccessToken,
            ExpiresInSeconds = totalSeconds
        };

        return serviceTokenResponse;
    }

    public static SignInResponse ToSignInResponse(this AccessAndRefreshTokenViewModel token)
    {
        var accessTokenRemainingTime = token.AccessTokenExpiresAt - DateTime.UtcNow;
        var refreshTokenRemainingTime = token.RefreshTokenExpiresAt - DateTime.UtcNow;

        return new SignInResponse
        {
            AccessToken = token.AccessToken,
            AccessTokenExpiresInSeconds = (int)accessTokenRemainingTime.TotalSeconds,
            RefreshToken = token.RefreshToken,
            RefreshTokenExpiresInSeconds = (int)refreshTokenRemainingTime.TotalSeconds
        };
    }
}
