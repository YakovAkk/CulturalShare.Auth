using CulturalShare.Auth.Domain.Context;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Auth.Repositories.Repositories.Base;
using CulturalShare.Common.DB;
using Microsoft.EntityFrameworkCore;

namespace CulturalShare.Auth.Repositories.Repositories;

public class AuthRepository : DbService<AuthDBContext>, IAuthRepository
{
    public AuthRepository(DbContextOptions<AuthDBContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public async Task<int> CreateUserAsync(UserEntity user)
    {
        using (var dbContext = CreateDbContext())
        {
            var result =  dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return result.Entity.Id;
        }
    }

    public async Task<UserEntity> GetUserByEmailAsync(string email)
    {
        using (var dbContext = CreateDbContext())
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }

    public async Task<UserEntity> GetUserByIdAsync(int id)
    {
        using (var dbContext = CreateDbContext())
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
