using AuthenticationProto;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;

namespace Service.Services.Base;

public interface IUserService
{
    Task<ErrorOr<Empty>> AllowUserAsync(AllowUserRequest request);
    Task<ErrorOr<int>> CreateUserAsync(CreateUserRequest request);
    Task<ErrorOr<Empty>> FollowUserAsync(FollowUserRequest request);
    Task<ErrorOr<Empty>> RestrictUserAsync(RestrictUserRequest request);
    Task<ErrorOr<SearchUserResponse>> SearchUserByNameAsync(SearchUserRequest request);
    Task<ErrorOr<Empty>> ToggleNotificationsAsync(ToggleNotificationsRequest request, HttpContext httpContext);
    Task<ErrorOr<Empty>> UnfollowUserAsync(UnfollowUserRequest request);
}
