using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Domain.Context;
using CulturalShare.Foundation.EntironmentHelper.EnvHelpers;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using StackExchange.Redis;

namespace CulturalShare.Auth.API.Configuration;

public class DatabaseServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var sortOutCredentialsHelper = new SortOutCredentialsHelper(builder.Configuration);

        builder.Services.AddDbContextPool<AuthDbContext>(options =>
             options.UseNpgsql(sortOutCredentialsHelper.GetPostgresConnectionString()));
        builder.Services.AddTransient<DbContext, AuthDbContext>();

        builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            return ConnectionMultiplexer.Connect(sortOutCredentialsHelper.GetRedisConnectionString());
        });

        logger.Information($"{nameof(DatabaseServiceInstaller)} installed.");
    }
}
