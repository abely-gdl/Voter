using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class LoginRequestDto
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}
