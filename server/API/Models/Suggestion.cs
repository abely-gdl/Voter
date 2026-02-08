namespace VoterAPI.Models;

public class Suggestion
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public required string Text { get; set; }
    public int SubmittedByUserId { get; set; }
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    public SuggestionStatus Status { get; set; } = SuggestionStatus.Pending;
    public bool IsVisible { get; set; } = true;

    // Navigation properties
    public Board? Board { get; set; }
    public User? SubmittedBy { get; set; }
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
