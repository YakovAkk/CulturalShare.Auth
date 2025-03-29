using CulturalShare.Foundation.EntironmentHelper.EnvHelpers;
using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Postgres.Infrastructure;
using Serilog.Core;
using StackExchange.Redis;

namespace Dependency.Infranstructure.Configuration;

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
