using AuthenticationProto;
using FluentValidation;

namespace Service.Validators;

public class UnfollowUserRequestValidator : AbstractValidator<UnfollowUserRequest>
{
    public UnfollowUserRequestValidator()
    {
        RuleFor(x => x.FolloweeId)
            .GreaterThan(0).WithMessage("Followee ID must be greater than 0.");
    }
}
