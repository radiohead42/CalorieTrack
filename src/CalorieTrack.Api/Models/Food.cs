namespace CalorieTrack.Api.Models;

public class Food
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Brand { get; set; }

    public string? ExternalId { get; set; }

    // Valores nutricionales por 100 g
    public decimal Calories { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbohydrates { get; set; }

    public decimal Fat { get; set; }

    public decimal Fiber { get; set; }

    public decimal Sugar { get; set; }

    public decimal Sodium { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
