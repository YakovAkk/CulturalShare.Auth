using AuthenticationProto;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;

namespace Service.Services.Base;

public interface IAuthService
{
    Task<ErrorOr<SignInResponse>> GetSignInAsync(SignInRequest request);
    Task<ErrorOr<ServiceTokenResponse>> GetServiceTokenAsync(ServiceTokenRequest request);
    Task<ErrorOr<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, int userId);
    Task<ErrorOr<Empty>> SignOutAsync(HttpContext httpContext);
}
