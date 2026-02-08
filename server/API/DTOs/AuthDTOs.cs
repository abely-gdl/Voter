using VoterAPI.Models;

namespace VoterAPI.DTOs;

public class LoginRequestDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class LoginResponseDto
{
    public required string Token { get; set; }
    public required UserDto User { get; set; }
}

public class RegisterRequestDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedDate { get; set; }
}
