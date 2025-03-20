using AuthenticationProto;
using Service.Model;

namespace CulturalShare.Auth.Services.Services.Base;

public interface IAuthService
{
    Task<AccessAndRefreshTokenViewModel> GetSignInAsync(SignInRequest request);
    Task<ServiceTokenResponse> GetServiceTokenAsync(ServiceTokenRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, int userId);
}
