using System.IdentityModel.Tokens.Jwt;
using CalorieTrack.Api.Data;
using CalorieTrack.Api.DTOs.Profile;
using CalorieTrack.Api.Models;
using CalorieTrack.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalorieTrack.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IValidator<UpdateProfileRequest> validator;
    private readonly CalorieCalculatorService calorieCalculator;

    public ProfileController(ApplicationDbContext context, IValidator<UpdateProfileRequest> validator,
            CalorieCalculatorService calorieCalculator)
    {
        this.context = context;
        this.validator = validator;
        this.calorieCalculator = calorieCalculator;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var userId = GetUserId();

        var profile = await context.UserProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
        {
            return NotFound(new { message = "Profile not found" });
        }

        return Ok(ToResponse(profile));
    }

    [HttpPut]
    public async Task<ActionResult<ProfileResponse>> UpdateProfile(
        UpdateProfileRequest request)
    {
        var validationResult =
            await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                validationResult.Errors.Select(x => new
                {
                    x.PropertyName,
                    x.ErrorMessage
                }));
        }

        var userId = GetUserId();

        var profile = await context.UserProfiles
            .SingleOrDefaultAsync(x => x.UserId == userId);

        if (profile is null)
        {
            profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            context.UserProfiles.Add(profile);
        }

        profile.HeightCm = request.HeightCm;
        profile.CurrentWeightKg = request.CurrentWeightKg;
        profile.DateOfBirth = request.DateOfBirth;
        profile.Sex = request.Sex;
        profile.ActivityLevel = request.ActivityLevel;
        profile.Goal = request.Goal;

        await context.SaveChangesAsync();

        return Ok(ToResponse(profile));
    }

    private Guid GetUserId()
    {
        var value = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(value, out var userId))
        {
            throw new Exception("Invalid user ID");
        }

        return userId;
    }

    private ProfileResponse ToResponse(UserProfile profile)
    {
        return new ProfileResponse
        {
        Id = profile.Id,
        HeightCm = profile.HeightCm,
        CurrentWeightKg = profile.CurrentWeightKg,
        DateOfBirth = profile.DateOfBirth,
        Sex = profile.Sex,
        ActivityLevel = profile.ActivityLevel,
        Goal = profile.Goal,
        Calories = calorieCalculator.Calculate(profile)
        };
    }

}
