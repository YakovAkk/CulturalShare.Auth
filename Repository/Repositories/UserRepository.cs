using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CulturalShare.Auth.Repositories.Repositories;

public class UserRepository : EntityFrameworkRepository<UserEntity>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }
}
