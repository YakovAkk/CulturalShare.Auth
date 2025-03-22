using AuthenticationProto;
using FluentValidation;

namespace Service.Validators;

public class ServiceTokenRequestValidator : AbstractValidator<ServiceTokenRequest>
{
    public ServiceTokenRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Service ID is required.");

        RuleFor(x => x.ServiceSecret)
            .NotEmpty().WithMessage("Service secret is required.");
    }
}
