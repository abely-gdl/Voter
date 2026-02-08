namespace VoterAPI.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<Suggestion> Suggestions { get; set; } = new List<Suggestion>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
