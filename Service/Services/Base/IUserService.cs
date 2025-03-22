using AuthenticationProto;
using ErrorOr;

namespace Service.Services.Base;

public interface IUserService
{
    Task<ErrorOr<int>> CreateUserAsync(CreateUserRequest request);
}
