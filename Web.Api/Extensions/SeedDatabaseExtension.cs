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
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
