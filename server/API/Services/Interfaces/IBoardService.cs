using VoterAPI.DTOs;

namespace VoterAPI.Services.Interfaces;

public interface IBoardService
{
    Task<IEnumerable<BoardDto>> GetAllBoardsAsync();
    Task<BoardDto?> GetBoardByIdAsync(int id);
    Task<BoardDetailDto?> GetBoardWithDetailsAsync(int id, int? currentUserId = null, bool isAdmin = false);
    Task<BoardDto> CreateBoardAsync(BoardCreateDto dto, int createdByUserId);
    Task<BoardDto?> UpdateBoardAsync(int id, BoardUpdateDto dto);
    Task<bool> ToggleVotingStatusAsync(int id);
    Task<bool> ToggleSuggestionsStatusAsync(int id);
    Task<bool> ToggleBoardStatusAsync(int id);
    Task<bool> DeleteBoardAsync(int id);
}
