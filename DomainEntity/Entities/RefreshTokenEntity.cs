using CulturalShare.Auth.Domain.Entities;
using MX.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities;

public class RefreshTokenEntity : BaseEntity<int>
{
    public string Token { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked { get; private set; }

    [ForeignKey(nameof(User))]
    public int UserEntityId { get; set; }

    [JsonIgnore]
    public UserEntity User { get; set; }

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    [NotMapped]
    public bool IsActive => !IsExpired && RevokedAt == null;

    public RefreshTokenEntity(string token, int secondsUntilExpire)
    {
        Token = token;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddSeconds(secondsUntilExpire);
    }
}
