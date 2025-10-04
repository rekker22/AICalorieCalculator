namespace AICalorieCalculator.Server.Models;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegistrationRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}