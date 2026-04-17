using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MicoHood.API.DTOs.Auth;
using MicoHood.API.Entities;
using MicoHood.API.Interfaces;

namespace MicoHood.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepo;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthRepository authRepo, IJwtService jwtService)
    {
        _authRepo = authRepo;
        _jwtService = jwtService;
    }

  
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _authRepo.EmailExistsAsync(dto.Email))
            return Conflict(new { message = "Email already in use" });

        if (await _authRepo.UsernameExistsAsync(dto.Username))
            return Conflict(new { message = "Username already taken" });

        var user = new User
        {
            Username = dto.Username.ToLower(),
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _authRepo.CreateUserAsync(user);

        return StatusCode(201, new RegisterResponseDto
        {
            Username = user.Username,
            Email = user.Email
        });
    }

 
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _authRepo.GetByEmailAsync(dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password" });

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            ExpiresAt = _jwtService.GetExpiry()
        });
    }
}