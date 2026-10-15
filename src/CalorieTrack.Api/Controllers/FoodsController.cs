using CalorieTrack.Api.DTOs;
using CalorieTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FoodsController : ControllerBase
{
    private readonly IFoodSearchService _foodSearchService;

    public FoodsController(
        IFoodSearchService foodSearchService)
    {
        _foodSearchService = foodSearchService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<
        IReadOnlyCollection<FoodSearchResult>>> Search(
        [FromQuery] string? query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) ||
            query.Trim().Length < 2)
        {
            return BadRequest(new
            {
                message =
                    "The search query must contain at least 2 characters."
            });
        }

        var results = await _foodSearchService.SearchAsync(
            query,
            cancellationToken);

        return Ok(results);
    }
}
