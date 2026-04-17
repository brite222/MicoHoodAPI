namespace MicoHood.API.DTOs.Auth;

public class RegisterResponseDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = "Registration successful. Please login.";
}