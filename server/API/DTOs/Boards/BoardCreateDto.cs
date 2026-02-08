using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class BoardCreateDto
{
    [Required]
    public required string Title { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    public bool IsSuggestionsOpen { get; set; } = true;
    public bool IsVotingOpen { get; set; } = true;
    public bool RequireApproval { get; set; } = false;
    
    [Required]
    public required string VotingType { get; set; } = "Single";
    
    public int? MaxVotes { get; set; }
}
