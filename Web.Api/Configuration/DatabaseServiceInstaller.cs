using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.Domain.Context;
using CulturalShare.Auth.Domain.Entities;
using CulturalShare.Common.Helper.EnvHelpers;
using Google.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using System;

namespace CulturalShare.Auth.API.Configuration;

public class DatabaseServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var sortOutCredentialsHelper = new SortOutCredentialsHelper(builder.Configuration);

        builder.Services.AddDbContextPool<AuthDbContext>(options =>
             options.UseNpgsql(sortOutCredentialsHelper.GetPostgresConnectionString()));
        builder.Services.AddTransient<DbContext, AuthDbContext>();

        logger.Information($"{nameof(DatabaseServiceInstaller)} installed.");
    }
}
