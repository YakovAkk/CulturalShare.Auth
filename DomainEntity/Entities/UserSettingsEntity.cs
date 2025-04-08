using MX.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities;

public class UserSettingsEntity : BaseEntity<int>
{
    private UserSettingsEntity()
    {

    }
    public UserSettingsEntity(bool notificationsEnabled, int userId)
    {
        NotificationsEnabled = notificationsEnabled;
        UserId = userId;
    }

    public bool NotificationsEnabled { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [JsonIgnore]
    public UserEntity User { get; set; }
}
