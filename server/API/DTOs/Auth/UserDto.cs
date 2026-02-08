namespace VoterAPI.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedDate { get; set; }
}
