using CulturalShare.Auth.Domain.Context;
using CulturalShare.Auth.Domain.Entities;

namespace CulturalShare.Auth.API.Extensions;

public static class SeedDatabaseExtension
{
    public static void SeedDatabase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                for (int i = 0; i < 1; i++)
                {
                    db.Users.Add(new UserEntity()
                    {
                        Email = "User@gmail.com",
                        FirstName = "FirstName",
                        LastName = "LastName",
                        PasswordHash = new byte[] { },
                    });

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
