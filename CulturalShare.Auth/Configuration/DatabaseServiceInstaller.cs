using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Domain.Context;
using CulturalShare.Common.Helper.EnvHelpers;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;

namespace CulturalShare.Auth.API.Configuration;

public class DatabaseServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var sortOutCredentialsHelper = new SortOutCredentialsHelper(builder.Configuration);

        builder.Services.AddDbContextPool<AuthDBContext>(options =>
             options.UseNpgsql(sortOutCredentialsHelper.DefaultConnectionString));

        builder.Services.AddTransient<DbContext, AuthDBContext>();

        logger.Information($"{nameof(DatabaseServiceInstaller)} installed.");
    }
}
