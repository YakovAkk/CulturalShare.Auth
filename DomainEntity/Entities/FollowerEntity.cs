using MX.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities;

public class FollowerEntity : BaseEntity<int>
{
    /// <summary>
    /// The user who is following
    /// </summary>
    [ForeignKey(nameof(Follower))]
    public int FollowerId { get; set; }      

    /// <summary>
    /// The user who is being followed
    /// </summary>
    [ForeignKey(nameof(Followee))]
    public int FolloweeId { get; set; }

    public DateTime? FollowedAt { get; set; } 

    public bool IsFollow { get; private set; }

    [JsonIgnore]
    public UserEntity? Follower { get; set; }

    [JsonIgnore]
    public UserEntity? Followee { get; set; }

    private FollowerEntity() { }

    public FollowerEntity(int followerId, int followeeId)
    {
        FollowerId = followerId;
        FolloweeId = followeeId;
        Follow();
    }

    public void Unfollow()
    {
       FollowedAt = null;
    }

    public void Follow()
    {
        FollowedAt = DateTime.UtcNow;
    }
}
