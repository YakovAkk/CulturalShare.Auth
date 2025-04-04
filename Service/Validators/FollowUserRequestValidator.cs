using AuthenticationProto;
using FluentValidation;

namespace Service.Validators;

public class FollowUserRequestValidator : AbstractValidator<FollowUserRequest>
{
    public FollowUserRequestValidator()
    {
        RuleFor(x => x.FolloweeId)
            .GreaterThan(0).WithMessage("Followee ID must be greater than 0.");
    }
}
