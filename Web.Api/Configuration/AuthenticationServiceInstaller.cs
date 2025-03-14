using CulturalShare.Auth.API.Configuration.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog.Core;
using Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Service.Services.Base;

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
                        ConfigureJwtBearerOptions(options, jwtSettings);
                    });

        builder.Services.AddAuthorization();

        logger.Information($"{nameof(AuthenticationServiceInstaller)} installed.");
    }

    private void ConfigureJwtBearerOptions(JwtBearerOptions options, JwtServicesConfig jwtSettings)
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = GetTokenValidationParameters(jwtSettings);
        options.Events = GetJwtBearerEvents();
    }

    private TokenValidationParameters GetTokenValidationParameters(JwtServicesConfig jwtSettings)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                ResolveIssuerSigningKey(token, jwtSettings),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            AudienceValidator = (audiences, securityToken, parameters) =>
                ValidateAudience(audiences, jwtSettings),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private SecurityKey[] ResolveIssuerSigningKey(string token, JwtServicesConfig jwtSettings)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = jwtHandler.ReadJwtToken(token);
        var serviceId = jwt.Audiences.FirstOrDefault();

        if (string.IsNullOrEmpty(serviceId) || !jwtSettings.JwtSecretTokenPairs.TryGetValue(serviceId, out var serviceSecret))
        {
            throw new SecurityTokenException("Invalid audience");
        }

        return new[] { new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceSecret)) };
    }

    private bool ValidateAudience(IEnumerable<string> audiences, JwtServicesConfig jwtSettings)
    {
        var isAudienceValid = audiences.Any(a => jwtSettings.JwtSecretTokenPairs.ContainsKey(a));
        return isAudienceValid;
    }

    private JwtBearerEvents GetJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnTokenValidated = async context => await ValidateTokenAsync(context)
        };
    }

    private async Task ValidateTokenAsync(TokenValidatedContext context)
    {
        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var blacklistService = context.HttpContext.RequestServices.GetRequiredService<IJwtBlacklistService>();

        var isTokenBlacklisted = await blacklistService.IsTokenBlacklistedAsync(jti);

        if (!string.IsNullOrEmpty(jti) && isTokenBlacklisted)
        {
            context.Fail("Token is revoked.");
        }
    } 
}

