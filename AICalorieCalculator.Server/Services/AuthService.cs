using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AICalorieCalculator.Server.Models;
using AICalorieCalculator.Server.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AICalorieCalculator.Server.Services;

public class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(UserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<(bool Success, string? Token, string? Message)> RegisterAsync(RegistrationRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return (false, null, "Passwords do not match");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userRepository.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return (true, GenerateJwtToken(user), "Registration successful");
        }
        return (false, null, string.Join(", ", result.Errors.Select(x => x.Description)));
    }

    public async Task<(bool Success, string? Token, string? Message)> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
            return (false, null, "Invalid email or password");

        var result = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!result)
            return (false, null, "Invalid email or password");

        return (true, GenerateJwtToken(user), "Login successful");
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

        var token = new JwtSecurityToken(
            _configuration["JwtIssuer"],
            _configuration["JwtIssuer"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
