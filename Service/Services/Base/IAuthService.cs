using AuthenticationProto;
using ErrorOr;
using Service.Model;

namespace Service.Services.Base;

public interface IAuthService
{
    Task<ErrorOr<SignInResponse>> GetSignInAsync(SignInRequest request);
    Task<ErrorOr<ServiceTokenResponse>> GetServiceTokenAsync(ServiceTokenRequest request);
    Task<ErrorOr<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, int userId);
}
