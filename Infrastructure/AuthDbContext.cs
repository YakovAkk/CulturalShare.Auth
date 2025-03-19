using CulturalShare.Auth.Domain.Entities;
using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CulturalShare.Auth.Domain.Context;

public class AuthDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshTokenEntity>()
            .Property(b => b.IsRevoked)
            .HasComputedColumnSql("CAST(CASE WHEN [RevokedAt] IS NOT NULL THEN 1 ELSE 0 END AS BIT)", stored: true);
    }
}
