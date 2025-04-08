using CulturalShare.Foundation.AspNetCore.Extensions.Constants;
using Dependency.Infranstructure.Configuration.Base;
using Dependency.Infranstructure.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Service.Validators;

namespace Dependency.Infranstructure.Configuration;

public class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder, Logger logger)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHeaderPropagation(options => options.Headers.Add(LoggingConsts.CorrelationIdHeaderName));

        builder.Services.AddControllers();
        builder.Services.AddServices();
        builder.Services.AddRepositories();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        logger.Information($"{nameof(ApplicationServiceInstaller)} installed.");
    }
}
