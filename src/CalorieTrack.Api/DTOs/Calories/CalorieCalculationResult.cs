namespace CalorieTrack.Api.DTOs.Calories;

public class CalorieCalculationResult
{
    public int Age { get; set; }
    public decimal BasalMetabolicRate { get; set; }
    public decimal MaintenanceCalories { get; set; }
    public decimal TargetCalories { get; set; }
}
