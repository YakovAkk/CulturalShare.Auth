using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace CulturalShare.Auth.API.Configuration;

public class DatabaseServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        var docker = builder.Configuration["DOTNET_RUNNING_IN_CONTAINER"];

        if (docker != null && docker.ToLower() == "true")
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgresDBDocker");

            Console.WriteLine(connectionString);
            builder.Services.AddDbContext<AuthDBContext>(options => options.UseNpgsql(connectionString));
        }
        else
        {
            Console.WriteLine(builder.Configuration.GetConnectionString("AuthDB"));
            builder.Services.AddDbContext<AuthDBContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDB")));
        }
    }
}
