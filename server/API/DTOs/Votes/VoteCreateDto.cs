using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class VoteCreateDto
{
    [Required]
    public int SuggestionId { get; set; }
}
