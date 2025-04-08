using AuthenticationProto;
using FluentValidation;

namespace Service.Validators;

public class RestrictUserRequestValidator : AbstractValidator<RestrictUserRequest>
{
    public RestrictUserRequestValidator()
    {
        RuleFor(x => x.UserIdToRestrict)
            .GreaterThan(0).WithMessage("User ID to restrict must be greater than 0.");
    }
}
