using AuthenticationProto;
using ErrorOr;
using Service.Model;

namespace CulturalShare.Auth.Services.Services.Base;

public interface IAuthService
{
    Task<ErrorOr<AccessAndRefreshTokenViewModel>> GetSignInAsync(SignInRequest request);
    Task<ErrorOr<ServiceTokenResponse>> GetServiceTokenAsync(ServiceTokenRequest request);
    Task<ErrorOr<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, int userId);
}
