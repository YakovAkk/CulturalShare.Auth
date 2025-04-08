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
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(e => e.RefreshTokens)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Followers)
               .WithOne(e => e.Followee)
               .HasForeignKey(e => e.FolloweeId)
               .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Following)
                .WithOne(f => f.Follower)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.RestrictedUsers)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity
            .Property(b => b.IsRevoked)
            .HasComputedColumnSql("\"RevokedAt\" IS NOT NULL", stored: true);

            entity.HasIndex(e => new { e.Token, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<FollowerEntity>(entity =>
        {
            entity
                .Property(b => b.IsFollow)
                .HasComputedColumnSql("\"FollowedAt\" IS NOT NULL", stored: true);

            entity.HasOne(f => f.Follower)
                .WithMany(u => u.Following) // Link to UserEntity.Following
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(f => f.Followee)
                .WithMany(u => u.Followers) // Link to UserEntity.Followers
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(e => new { e.FollowerId, e.FolloweeId }).IsUnique();
        });

        modelBuilder.Entity<RestrictedUserEntity>(entity =>
        {
            entity
                .Property(b => b.IsRestricted)
                .HasComputedColumnSql("\"RestrictedAt\" IS NOT NULL", stored: true);

            entity.HasOne(r => r.User)
                .WithMany(u => u.RestrictedUsers) // Users this user has restricted
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(e => new { e.RestrictedUserId, e.UserId }).IsUnique();
        });
    }
}
