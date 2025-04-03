using CulturalShare.Repositories;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

namespace Repositories.Infrastructure.Repositories;

public class UserSettingsRepository : EntityFrameworkRepository<UserSettingsEntity>, IUserSettingsRepository
{
    public UserSettingsRepository(DbContext context) : base(context)
    {
    }
}
