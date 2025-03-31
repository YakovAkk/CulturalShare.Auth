using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Postgres.Infrastructure;

public class AuthDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.Property(b => b.IsRevoked)
            .HasComputedColumnSql("\"RevokedAt\" IS NOT NULL", stored: true);

            entity.HasIndex(e => new { e.Token, e.UserId }).IsUnique();
        });
    }
}
