using CulturalShare.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CulturalShare.Auth.Domain.Context;

public class AuthDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
}
