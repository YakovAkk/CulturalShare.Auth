using AuthenticationBackProto;
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
}
