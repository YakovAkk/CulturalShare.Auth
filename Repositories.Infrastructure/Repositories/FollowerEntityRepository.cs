using CulturalShare.Repositories;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

namespace Repositories.Infrastructure.Repositories;

public class FollowerEntityRepository : EntityFrameworkRepository<FollowerEntity>, IFollowerEntityRepository
{
    public FollowerEntityRepository(DbContext context) : base(context)
    {
    }
}
