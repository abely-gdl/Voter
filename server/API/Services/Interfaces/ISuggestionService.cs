using VoterAPI.DTOs;

namespace VoterAPI.Services.Interfaces;

public interface ISuggestionService
{
    Task<IEnumerable<SuggestionDto>> GetSuggestionsByBoardIdAsync(int boardId);
    Task<SuggestionDto?> GetSuggestionByIdAsync(int id);
    Task<SuggestionDto> CreateSuggestionAsync(int boardId, SuggestionCreateDto dto, int submittedByUserId);
    Task<IEnumerable<SuggestionDto>> GetPendingSuggestionsAsync();
    Task<bool> ApproveSuggestionAsync(int id);
    Task<bool> RejectSuggestionAsync(int id);
    Task<bool> DeleteSuggestionAsync(int id);
}
