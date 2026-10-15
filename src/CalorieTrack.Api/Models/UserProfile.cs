using CalorieTrack.Api.Models.Enums;

namespace CalorieTrack.Api.Models;

public class UserProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal HeightCm { get; set; }

    public decimal CurrentWeightKg { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Sex Sex { get; set; }

    public ActivityLevel ActivityLevel { get; set; }

    public GoalType Goal { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
