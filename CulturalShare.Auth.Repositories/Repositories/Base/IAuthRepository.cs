
using CulturalShare.Auth.Domain.Entities;

namespace CulturalShare.Auth.Repositories.Repositories.Base;

public interface IAuthRepository
{
    Task<int> CreateUserAsync(UserEntity user);
    Task<UserEntity> GetUserByEmailAsync(string email);
    Task<UserEntity> GetUserByIdAsync(int id);
}
