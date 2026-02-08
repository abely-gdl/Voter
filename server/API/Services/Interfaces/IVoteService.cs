using VoterAPI.DTOs;

namespace VoterAPI.Services.Interfaces;

public interface IVoteService
{
    Task<VoteDto> AddVoteAsync(int suggestionId, int userId);
    Task<bool> RemoveVoteAsync(int suggestionId, int userId);
    Task<IEnumerable<VoteDto>> GetVotesBySuggestionIdAsync(int suggestionId);
    Task<IEnumerable<VoteDto>> GetUserVotesByBoardIdAsync(int userId, int boardId);
}
