using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities;

public class RestrictedUserEntity
{
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [ForeignKey(nameof(RestrictedUser))]
    public int RestrictedUserId { get; set; }

    public DateTime? RestrictedAt { get; set; }
    public bool IsRestricted { get; private set; }

    [JsonIgnore]
    public UserEntity User { get; set; }

    [JsonIgnore]
    public UserEntity RestrictedUser { get; set; }

    private RestrictedUserEntity() { }

    public RestrictedUserEntity(int userId, int restrictedUserId)
    {
        UserId = userId;
        RestrictedUserId = restrictedUserId;
    }

    public void Restrict()
    {
        RestrictedAt = DateTime.UtcNow;
    }

    public void Unrestrict()
    {
        RestrictedAt = null;
    }
}
