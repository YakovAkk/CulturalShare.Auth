using AuthenticationProto;
using FluentValidation;

namespace Service.Validators;

public class AllowUserRequestValidator : AbstractValidator<AllowUserRequest>
{
    public AllowUserRequestValidator()
    {
        RuleFor(x => x.UserIdToAllowObserving)
            .GreaterThan(0).WithMessage("User ID to allow must be greater than 0.");
    }
}