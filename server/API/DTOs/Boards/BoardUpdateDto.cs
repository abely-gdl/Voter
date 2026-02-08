using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class BoardUpdateDto
{
    [Required]
    public required string Title { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    public bool IsSuggestionsOpen { get; set; }
    public bool IsVotingOpen { get; set; }
    public bool RequireApproval { get; set; }
    
    [Required]
    public required string VotingType { get; set; }
    
    public int? MaxVotes { get; set; }
}
