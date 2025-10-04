using AICalorieCalculator.Server.Models;
using AICalorieCalculator.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AICalorieCalculator.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegistrationRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.Success)
        {
            return Ok(new AuthResponse { Success = true, Token = result.Token, Message = result.Message });
        }
        return BadRequest(new AuthResponse { Success = false, Message = result.Message });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result.Success)
        {
            return Ok(new AuthResponse { Success = true, Token = result.Token, Message = result.Message });
        }
        return Unauthorized(new AuthResponse { Success = false, Message = result.Message });
    }
}