namespace VoterAPI.DTOs;

public class VoteDto
{
    public int Id { get; set; }
    public int SuggestionId { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public DateTime VotedDate { get; set; }
}

public class VoteCreateDto
{
    public int SuggestionId { get; set; }
}
