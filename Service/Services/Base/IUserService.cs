using AuthenticationProto;

namespace Service.Services.Base;

public interface IUserService
{
    Task<int> CreateUserAsync(CreateUserRequest request);
}
