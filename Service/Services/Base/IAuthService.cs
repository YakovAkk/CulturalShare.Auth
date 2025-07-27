using AuthenticationBackProto;
using AuthenticationProto;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;

namespace Service.Services.Base;

public interface IAuthService
{
    Task<ErrorOr<ServiceTokenResponse>> GetServiceTokenAsync(ServiceTokenRequest request);
    Task<ErrorOr<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, int userId);
    Task<ErrorOr<UserTokenResponse>> GenerateUserTokenAsync(UserTokenRequest request);
    Task<ErrorOr<Empty>> RevokeUserTokenAsync(int userId);
}
