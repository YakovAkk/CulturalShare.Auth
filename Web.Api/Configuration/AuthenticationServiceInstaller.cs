using CulturalShare.Auth.API.Configuration.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog.Core;
using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Google.Api;
using Microsoft.Extensions.DependencyInjection;

namespace CulturalShare.Auth.API.Configuration;

public class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        var jwtSettings = builder.Configuration.GetSection("JwtServicesConfig").Get<JwtServicesConfig>();
        builder.Services.AddSingleton(jwtSettings);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var jwt = jwtHandler.ReadJwtToken(token);
                        var serviceId = jwt.Audiences.FirstOrDefault(); // Extract Audience as Service ID

                        if (string.IsNullOrEmpty(serviceId) || !jwtSettings.JwtServices.TryGetValue(serviceId, out var serviceSecret))
                        {
                            throw new SecurityTokenException("Invalid audience");
                        }

                        return new[] { new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceSecret)) };
                    },
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    AudienceValidator = (audiences, securityToken, parameters) =>
                    {
                        var validAudience = jwtSettings.JwtServices.TryGetValue(audiences.First(), out var _);
                        return validAudience;
                    },
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        //var blacklistService = context.HttpContext.RequestServices.GetRequiredService<JwtBlacklistService>();

                        //if (!string.IsNullOrEmpty(jti) && await blacklistService.IsTokenBlacklistedAsync(jti))
                        //{
                        //    context.Fail("Token is revoked.");
                        //}

                        if (string.IsNullOrEmpty(jti))
                        {
                            context.Fail("Token is revoked.");
                        }
                    }
                };
            });

        builder.Services.AddAuthorization();

        logger.Information($"{nameof(AuthenticationServiceInstaller)} installed.");
    }
}

