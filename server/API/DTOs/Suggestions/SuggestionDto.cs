using System.ComponentModel.DataAnnotations;

namespace VoterAPI.DTOs;

public class SuggestionDto
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public required string Text { get; set; }
    public int SubmittedByUserId { get; set; }
    public string? SubmittedByUsername { get; set; }
    public DateTime SubmittedDate { get; set; }
    public required string Status { get; set; }
    public bool IsVisible { get; set; }
}
