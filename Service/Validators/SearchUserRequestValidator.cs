using AuthenticationProto;
using FluentValidation;

namespace Service.Validators;

public class SearchUserRequestValidator : AbstractValidator<SearchUserRequest>
{
    public SearchUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name must not be empty.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");
    }
}
