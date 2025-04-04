using DomainEntity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Postgres.Infrastructure;

public class AuthDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<FollowerEntity> Followers { get; set; }
    public DbSet<UserSettingsEntity> UserSettings { get; set; }
    public DbSet<RestrictedUserEntity> RestrictedUsers { get; set; }

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

        modelBuilder.Entity<FollowerEntity>()
            .Property(b => b.IsFollow)
            .HasComputedColumnSql("\"FollowedAt\" IS NOT NULL", stored: true);

        modelBuilder.Entity<RestrictedUserEntity>()
           .Property(b => b.IsRestricted)
           .HasComputedColumnSql("\"RestrictedAt\" IS NULL", stored: true);
    }
}
