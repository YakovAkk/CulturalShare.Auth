using CulturalShare.Repositories;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Base;

namespace Repository.Repositories;

public class RefreshTokenRepository : EntityFrameworkRepository<RefreshTokenEntity>, IRefreshTokenRepository
{
    public RefreshTokenRepository(DbContext context) : base(context)
    {
    }
}
