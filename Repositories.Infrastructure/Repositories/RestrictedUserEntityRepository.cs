using CulturalShare.Repositories;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

namespace Repositories.Infrastructure.Repositories;

public class RestrictedUserEntityRepository : EntityFrameworkRepository<RestrictedUserEntity>, IRestrictedUserEntityRepository
{
    public RestrictedUserEntityRepository(DbContext context) : base(context)
    {
    }
}
