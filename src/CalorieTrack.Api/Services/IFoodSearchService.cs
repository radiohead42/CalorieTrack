using CalorieTrack.Api.DTOs;

namespace CalorieTrack.Api.Services;

public interface IFoodSearchService
{
    Task<IReadOnlyCollection<FoodSearchResult>> SearchAsync(
            string query, CancellationToken cancellationToken = default);
}
