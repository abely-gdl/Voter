namespace VoterAPI.Models;

public class Board
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }
    public bool IsSuggestionsOpen { get; set; } = true;
    public bool IsVotingOpen { get; set; } = true;
    public bool RequireApproval { get; set; } = false;
    public VotingType VotingType { get; set; } = VotingType.Single;
    public int? MaxVotes { get; set; }
    public bool IsClosed { get; set; } = false;

    // Navigation properties
    public User? CreatedBy { get; set; }
    public ICollection<Suggestion> Suggestions { get; set; } = new List<Suggestion>();
}
