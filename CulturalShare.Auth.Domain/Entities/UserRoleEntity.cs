using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using CulturalShare.Auth.Domain.Enums;

namespace CulturalShare.Auth.Domain.Entities;

public class UserRoleEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public virtual UserEntity Administrator { get; set; }
    public Role Role { get; set; }
}