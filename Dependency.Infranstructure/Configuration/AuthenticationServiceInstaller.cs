using CulturalShare.Foundation.Authorization.AuthenticationExtension;
using CulturalShare.Foundation.EnvironmentHelper.EnvHelpers;
using Dependency.Infranstructure.Configuration.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;

namespace Dependency.Infranstructure.Configuration;

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

