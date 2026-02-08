using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.DTOs;
using VoterAPI.Models;

namespace VoterAPI.Services;

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

public class SuggestionService : ISuggestionService
{
    private readonly ISuggestionRepository _suggestionRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IMapper _mapper;

    public SuggestionService(
        ISuggestionRepository suggestionRepository,
        IBoardRepository boardRepository,
        IMapper mapper)
    {
        _suggestionRepository = suggestionRepository;
        _boardRepository = boardRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SuggestionDto>> GetSuggestionsByBoardIdAsync(int boardId)
    {
        var suggestions = await _suggestionRepository.GetSuggestionsByBoardIdAsync(boardId);
        return _mapper.Map<IEnumerable<SuggestionDto>>(suggestions);
    }

    public async Task<SuggestionDto?> GetSuggestionByIdAsync(int id)
    {
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        return suggestion == null ? null : _mapper.Map<SuggestionDto>(suggestion);
    }

    public async Task<SuggestionDto> CreateSuggestionAsync(int boardId, SuggestionCreateDto dto, int submittedByUserId)
    {
        // Verify board exists and suggestions are open
        var board = await _boardRepository.GetByIdAsync(boardId);
        if (board == null)
        {
            throw new InvalidOperationException("Board not found");
        }

        if (!board.IsSuggestionsOpen)
        {
            throw new InvalidOperationException("Suggestions are not currently open for this board");
        }

        var suggestion = _mapper.Map<Suggestion>(dto);
        suggestion.BoardId = boardId;
        suggestion.SubmittedByUserId = submittedByUserId;
        suggestion.SubmittedDate = DateTime.UtcNow;
        suggestion.Status = board.RequireApproval ? SuggestionStatus.Pending : SuggestionStatus.Approved;

        await _suggestionRepository.AddAsync(suggestion);
        await _suggestionRepository.SaveChangesAsync();

        return _mapper.Map<SuggestionDto>(suggestion);
    }

    public async Task<IEnumerable<SuggestionDto>> GetPendingSuggestionsAsync()
    {
        var suggestions = await _suggestionRepository.GetPendingSuggestionsAsync();
        return _mapper.Map<IEnumerable<SuggestionDto>>(suggestions);
    }

    public async Task<bool> ApproveSuggestionAsync(int id)
    {
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        if (suggestion == null) return false;

        suggestion.Status = SuggestionStatus.Approved;
        _suggestionRepository.Update(suggestion);
        await _suggestionRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectSuggestionAsync(int id)
    {
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        if (suggestion == null) return false;

        suggestion.Status = SuggestionStatus.Rejected;
        suggestion.IsVisible = false;
        _suggestionRepository.Update(suggestion);
        await _suggestionRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteSuggestionAsync(int id)
    {
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        if (suggestion == null) return false;

        _suggestionRepository.Delete(suggestion);
        await _suggestionRepository.SaveChangesAsync();

        return true;
    }
}
