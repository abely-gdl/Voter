using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class BoardDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedByUserId { get; set; }
    public string? CreatedByUsername { get; set; }
    public bool IsSuggestionsOpen { get; set; }
    public bool IsVotingOpen { get; set; }
    public bool RequireApproval { get; set; }
    public required string VotingType { get; set; }
    public int? MaxVotes { get; set; }
    public bool IsClosed { get; set; }
    public int SuggestionCount { get; set; }
    public int TotalVotes { get; set; }
}
