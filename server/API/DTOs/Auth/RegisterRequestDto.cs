using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class RegisterRequestDto
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}
