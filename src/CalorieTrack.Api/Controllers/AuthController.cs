using CalorieTrack.Api.DTOs;
using CalorieTrack.Api.DTOs.Auth;
using CalorieTrack.Api.Models;
using CalorieTrack.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IValidator<RegisterRequest> registerValidator;
    private readonly IValidator<LoginRequest> loginValidator;
    private readonly JwService jwService;

    public AuthController(UserManager<ApplicationUser> userManager, IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator, JwService jwService)
    {
        this.userManager = userManager;
        this.registerValidator = registerValidator;
        this.loginValidator = loginValidator;
        this.jwService = jwService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var validationResult =
            await registerValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => new
                        {
                        error.PropertyName,
                        error.ErrorMessage
                        });

            return BadRequest(errors);
        }

        var existingUser =
            await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Conflict(new
                    {
                    message = "A user with this email already exists."
                    });
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email
        };

        var result =
            await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status201Created, new
                {
                user.Id,
                user.Email
                });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var validationResult =
            await loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => new
                        {
                        error.PropertyName,
                        error.ErrorMessage
                        });

            return BadRequest(errors);
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new
                    {
                    message = "Invalid email or password."
                    });
        }

        var passwordIsValid =
            await userManager.CheckPasswordAsync(
                    user,
                    request.Password);

        if (!passwordIsValid)
        {
            return Unauthorized(new
                    {
                    message = "Invalid email or password."
                    });
        }

        var token = jwService.GenerateToken(user);

        return Ok(new
                {
                token,
                user = new
                {
                user.Id,
                user.Email
                }
                });
    }
}
