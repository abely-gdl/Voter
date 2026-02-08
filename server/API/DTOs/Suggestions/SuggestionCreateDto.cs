using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class SuggestionCreateDto
{
    [Required]
    public required string Text { get; set; }
}
