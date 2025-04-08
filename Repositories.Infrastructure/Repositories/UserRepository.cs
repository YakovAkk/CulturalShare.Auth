using CulturalShare.Repositories;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

namespace Repositories.Infrastructure.Repositories;

public class UserRepository : EntityFrameworkRepository<UserEntity>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }
}
