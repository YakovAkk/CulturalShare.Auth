using CulturalShare.Auth.Domain.Context;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Common.DB;
using Microsoft.EntityFrameworkCore;
using Monto.Repositories;

namespace CulturalShare.Auth.Repositories.Repositories;

public class AuthRepository : EntityFrameworkRepository<UserEntity>, IAuthRepository
{
    public AuthRepository(DbContext context) : base(context)
    {
    }
}
