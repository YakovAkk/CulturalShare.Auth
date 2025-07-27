using AuthenticationBackProto;
using FluentValidation;

namespace Service.Validators;

public class UserTokenRequestValidator : AbstractValidator<UserTokenRequest>
{
    public UserTokenRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than zero.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");
    }
}
