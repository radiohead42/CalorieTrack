using CalorieTrack.Api.Models.Enums;

namespace CalorieTrack.Api.DTOs.Profile;

public class UpdateProfileRequest
{
    public decimal HeightCm { get; set; }
    public decimal CurrentWeightKg { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Sex Sex { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public GoalType Goal { get; set; }
}
