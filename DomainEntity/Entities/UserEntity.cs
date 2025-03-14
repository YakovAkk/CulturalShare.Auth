using MX.Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace CulturalShare.Auth.Domain.Entities;

public class UserEntity : BaseEntity<int>
{
    [MaxLength(200)]
    public string FirstName { get; set; }

    [MaxLength(200)]
    public string LastName { get; set; }

    [MaxLength(200)]
    public string Email { get; set; }

    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}
