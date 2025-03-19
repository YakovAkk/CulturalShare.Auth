using CulturalShare.Auth.API.Configuration.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog.Core;
using CulturalShare.Foundation.Authorization.AuthenticationExtension;
using CulturalShare.Foundation.EntironmentHelper.Configurations;
using CulturalShare.Foundation.EntironmentHelper.EnvHelpers;

namespace CulturalShare.Auth.API.Configuration;

public class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var sortOutCredentialsHelper = new SortOutCredentialsHelper(builder.Configuration);

        var jwtSettings = sortOutCredentialsHelper.GetJwtServicesConfiguration();
        builder.Services.AddSingleton(jwtSettings);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        JwtExtension.ConfigureJwtBearerOptions(options, jwtSettings);
                    });

        builder.Services.AddAuthorization();

        logger.Information($"{nameof(AuthenticationServiceInstaller)} installed.");
    }
}

