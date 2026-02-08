namespace VoterAPI.Models;

public class Vote
{
    public int Id { get; set; }
    public int SuggestionId { get; set; }
    public int UserId { get; set; }
    public DateTime VotedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Suggestion? Suggestion { get; set; }
    public User? User { get; set; }
}
