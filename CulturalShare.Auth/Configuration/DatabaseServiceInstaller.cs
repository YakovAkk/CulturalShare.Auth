using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Domain.Context;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;

namespace CulturalShare.Auth.API.Configuration;

public class DatabaseServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var docker = builder.Configuration["DOTNET_RUNNING_IN_CONTAINER"];

        if (docker != null && docker.ToLower() == "true")
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgresDBDocker");

            logger.Information(connectionString);

            builder.Services.AddDbContext<AuthDBContext>(options => options.UseNpgsql(connectionString));
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            logger.Information(connectionString);

            builder.Services.AddDbContextPool<AuthDBContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddTransient<DbContext, AuthDBContext>();
        }
    }
}
