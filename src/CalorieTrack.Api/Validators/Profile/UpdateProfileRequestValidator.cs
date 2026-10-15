using CalorieTrack.Api.DTOs.Profile;
using CalorieTrack.Api.Models.Enums;
using FluentValidation;

namespace CalorieTrack.Api.Validators.Profile;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.HeightCm)
            .InclusiveBetween(50, 300);

        RuleFor(x => x.CurrentWeightKg)
            .InclusiveBetween(20, 500);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.Sex)
            .IsInEnum();

        RuleFor(x => x.ActivityLevel)
            .IsInEnum();

        RuleFor(x => x.Goal)
            .IsInEnum();
    }
}
