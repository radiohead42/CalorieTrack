using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalorieTrack.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace CalorieTrack.Api.Services;

public class JwService
{
    private readonly IConfiguration configuration;

    public JwService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GenerateToken(ApplicationUser user)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");

        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");

        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");

        var expirationMinutes =
            configuration.GetValue<int>("Jwt:ExpirationMinutes");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
