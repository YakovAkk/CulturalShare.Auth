using AuthenticationProto;
using CulturalShare.Auth.Services.Model;

namespace CulturalShare.Auth.Services.Services.Base;

public interface IAuthService
{
    Task<int> CreateUserAsync(CreateUserRequest request);
    Task<AccessKeyViewModel> GetAccessTokenAsync(SignInRequest request);
    ServiceTokenResponse GetServiceTokenAsync(ServiceTokenRequest request);
}
