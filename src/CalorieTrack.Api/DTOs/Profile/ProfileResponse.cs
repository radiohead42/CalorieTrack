using CalorieTrack.Api.DTOs.Calories;
using CalorieTrack.Api.Models.Enums;

namespace CalorieTrack.Api.DTOs.Profile;

public class ProfileResponse
{
    public Guid Id { get; set; }

    public decimal HeightCm { get; set; }

    public decimal CurrentWeightKg { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Sex Sex { get; set; }

    public ActivityLevel ActivityLevel { get; set; }

    public GoalType Goal { get; set; }

    public CalorieCalculationResult Calories { get; set; } = null!;
}
