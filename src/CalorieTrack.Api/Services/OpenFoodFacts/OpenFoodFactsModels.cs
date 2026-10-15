using System.Text.Json.Serialization;

namespace CalorieTrack.Api.Services.OpenFoodFacts;

internal sealed class OpenFoodFactsSearchResponse
{
    [JsonPropertyName("hits")]
    public List<OpenFoodFactsProduct> Hits { get; set; } = [];
}

internal sealed class OpenFoodFactsProduct
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("brands")]
    public List<string> Brands { get; set; } = [];

    [JsonPropertyName("nutriments")]
    public OpenFoodFactsNutriments? Nutriments { get; set; }
}

internal sealed class OpenFoodFactsNutriments
{
    [JsonPropertyName("energy-kcal_100g")]
    public decimal? Calories { get; set; }

    [JsonPropertyName("proteins_100g")]
    public decimal? Protein { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public decimal? Carbohydrates { get; set; }

    [JsonPropertyName("fat_100g")]
    public decimal? Fat { get; set; }

    [JsonPropertyName("fiber_100g")]
    public decimal? Fiber { get; set; }

    [JsonPropertyName("sugars_100g")]
    public decimal? Sugar { get; set; }

    [JsonPropertyName("sodium_100g")]
    public decimal? Sodium { get; set; }
}

internal sealed class OpenFoodFactsSearchRequest
{
    [JsonPropertyName("q")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; } = 10;

    [JsonPropertyName("fields")]
    public string[] Fields { get; set; } =
    [
        "code",
        "product_name",
        "brands",
        "nutriments"
    ];
}
