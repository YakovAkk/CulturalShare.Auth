using CulturalShare.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CulturalShare.Auth.Domain.Context;

public class AuthDBContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options)
    {
        try
        {
            Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}
