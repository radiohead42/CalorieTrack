using CalorieTrack.Api.DTOs.Calories;
using CalorieTrack.Api.Models;
using CalorieTrack.Api.Models.Enums;

namespace CalorieTrack.Api.Services;

public class CalorieCalculatorService
{
    private const decimal WeightGoalAdjustment = 500m;

    public CalorieCalculationResult Calculate(UserProfile profile)
    {
        var age = CalculateAge(profile.DateOfBirth);

        var sexAdjustment = profile.Sex switch
        {
            Sex.Male => 5m,
            Sex.Female => -161m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(profile.Sex))
        };

        var bmr =
            (10m * profile.CurrentWeightKg) +
            (6.25m * profile.HeightCm) -
            (5m * age) +
            sexAdjustment;

        var activityFactor = profile.ActivityLevel switch
        {
            ActivityLevel.Sedentary => 1.2m,
            ActivityLevel.LightlyActive => 1.375m,
            ActivityLevel.ModeratelyActive => 1.55m,
            ActivityLevel.VeryActive => 1.725m,
            ActivityLevel.ExtraActive => 1.9m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(profile.ActivityLevel))
        };

        var maintenanceCalories = bmr * activityFactor;

        var targetCalories = profile.Goal switch
        {
            GoalType.LoseWeight =>
                maintenanceCalories - WeightGoalAdjustment,

            GoalType.MaintainWeight =>
                maintenanceCalories,

            GoalType.GainWeight =>
                maintenanceCalories + WeightGoalAdjustment,

            _ => throw new ArgumentOutOfRangeException(
                nameof(profile.Goal))
        };

        return new CalorieCalculationResult
        {
            Age = age,

            BasalMetabolicRate =
                Math.Round(bmr),

            MaintenanceCalories =
                Math.Round(maintenanceCalories),

            TargetCalories =
                Math.Round(targetCalories)
        };
    }

    private static int CalculateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age;

    }
}
