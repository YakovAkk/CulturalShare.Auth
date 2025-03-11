using AuthenticationProto;
using CulturalShare.Auth.Services.Model;

namespace CulturalShare.Auth.Services.Services.Base;

public interface IAuthService
{
    Task<int> CreateUserAsync(CreateUserRequest request);
    Task<AccessKeyViewModel> GetAccessTokenAsync(LoginRequest request);
    AccessKeyViewModel GetOneTimeTokenAsync(GetOneTimeTokenRequest request);
}
