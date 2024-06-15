using CulturalShare.Auth.API.Configuration.Base;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Serilog.Core;

namespace CulturalShare.Auth.API.Configuration;

public class SwaggerServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth API",
                Version = "v1",
            });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        logger.Information($"{nameof(HealthCheckServiceInstaller)} installed.");
    }
}
