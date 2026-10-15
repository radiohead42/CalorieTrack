using System.Net.Http.Json;
using CalorieTrack.Api.DTOs;

namespace CalorieTrack.Api.Services.OpenFoodFacts;

public sealed class OpenFoodFactsService : IFoodSearchService
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<FoodSearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var request = new OpenFoodFactsSearchRequest
        {
            Query = query.Trim(),
            Page = 1,
            PageSize = 10
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "search",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var searchResponse =
            await response.Content.ReadFromJsonAsync<OpenFoodFactsSearchResponse>(
                cancellationToken: cancellationToken);

        if (searchResponse is null)
        {
            return [];
        }

        return searchResponse.Hits
            .Where(product =>
                !string.IsNullOrWhiteSpace(product.Code) &&
                !string.IsNullOrWhiteSpace(product.ProductName) &&
                product.Nutriments is not null)
            .Select(MapProduct)
            .ToArray();
    }

    private static FoodSearchResult MapProduct(
        OpenFoodFactsProduct product)
    {
        var nutrients = product.Nutriments!;

        return new FoodSearchResult
        {
            ExternalId = product.Code!,
            Name = product.ProductName!,
            Brand = product.Brands.FirstOrDefault(),

            Calories = nutrients.Calories ?? 0,
            Protein = nutrients.Protein ?? 0,
            Carbohydrates = nutrients.Carbohydrates ?? 0,
            Fat = nutrients.Fat ?? 0,
            Fiber = nutrients.Fiber ?? 0,
            Sugar = nutrients.Sugar ?? 0,
            Sodium = nutrients.Sodium ?? 0
        };
    }
}
