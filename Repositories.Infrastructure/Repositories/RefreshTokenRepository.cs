using CulturalShare.Repositories;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

namespace Repositories.Infrastructure.Repositories;

public class RefreshTokenRepository : EntityFrameworkRepository<RefreshTokenEntity>, IRefreshTokenRepository
{
    public RefreshTokenRepository(DbContext context) : base(context)
    {
    }
}
