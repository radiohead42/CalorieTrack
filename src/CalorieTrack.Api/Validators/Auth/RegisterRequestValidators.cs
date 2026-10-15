using CalorieTrack.Api.DTOs.Auth;
using FluentValidation;

namespace CalorieTrack.Api.Validators.Auth;

public class RegisterRequestValidators : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidators()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
