using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.DTOs;
using VoterAPI.Models;

namespace VoterAPI.Services;

public interface IBoardService
{
    Task<IEnumerable<BoardDto>> GetAllBoardsAsync();
    Task<BoardDto?> GetBoardByIdAsync(int id);
    Task<BoardDetailDto?> GetBoardWithDetailsAsync(int id, int? currentUserId = null);
    Task<BoardDto> CreateBoardAsync(BoardCreateDto dto, int createdByUserId);
    Task<BoardDto?> UpdateBoardAsync(int id, BoardUpdateDto dto);
    Task<bool> ToggleVotingAsync(int id);
    Task<bool> ToggleSuggestionsAsync(int id);
    Task<bool> CloseBoardAsync(int id);
    Task<bool> DeleteBoardAsync(int id);
}

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IMapper _mapper;

    public BoardService(IBoardRepository boardRepository, IMapper mapper)
    {
        _boardRepository = boardRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardDto>> GetAllBoardsAsync()
    {
        var boards = await _boardRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<BoardDto>>(boards);
    }

    public async Task<BoardDto?> GetBoardByIdAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        return board == null ? null : _mapper.Map<BoardDto>(board);
    }

    public async Task<BoardDetailDto?> GetBoardWithDetailsAsync(int id, int? currentUserId = null)
    {
        var board = await _boardRepository.GetBoardWithDetailsAsync(id);
        if (board == null) return null;

        var boardDetail = _mapper.Map<BoardDetailDto>(board);

        // Filter suggestions based on status and set UserHasVoted
        var approvedSuggestions = board.Suggestions
            .Where(s => s.Status == SuggestionStatus.Approved && s.IsVisible)
            .ToList();

        boardDetail.Suggestions = approvedSuggestions.Select(s =>
        {
            var suggestionDto = _mapper.Map<SuggestionWithVotesDto>(s);
            suggestionDto.UserHasVoted = currentUserId.HasValue && 
                s.Votes.Any(v => v.UserId == currentUserId.Value);
            return suggestionDto;
        }).ToList();

        return boardDetail;
    }

    public async Task<BoardDto> CreateBoardAsync(BoardCreateDto dto, int createdByUserId)
    {
        var board = _mapper.Map<Board>(dto);
        board.CreatedByUserId = createdByUserId;
        board.CreatedDate = DateTime.UtcNow;

        await _boardRepository.AddAsync(board);
        await _boardRepository.SaveChangesAsync();

        return _mapper.Map<BoardDto>(board);
    }

    public async Task<BoardDto?> UpdateBoardAsync(int id, BoardUpdateDto dto)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return null;

        if (dto.Title != null) board.Title = dto.Title;
        if (dto.Description != null) board.Description = dto.Description;
        if (dto.IsSuggestionsOpen.HasValue) board.IsSuggestionsOpen = dto.IsSuggestionsOpen.Value;
        if (dto.IsVotingOpen.HasValue) board.IsVotingOpen = dto.IsVotingOpen.Value;
        if (dto.RequireApproval.HasValue) board.RequireApproval = dto.RequireApproval.Value;
        if (dto.VotingType != null) board.VotingType = Enum.Parse<VotingType>(dto.VotingType);
        if (dto.MaxVotes.HasValue) board.MaxVotes = dto.MaxVotes;
        if (dto.IsClosed.HasValue) board.IsClosed = dto.IsClosed.Value;

        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return _mapper.Map<BoardDto>(board);
    }

    public async Task<bool> ToggleVotingAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        board.IsVotingOpen = !board.IsVotingOpen;
        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleSuggestionsAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        board.IsSuggestionsOpen = !board.IsSuggestionsOpen;
        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CloseBoardAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        board.IsClosed = true;
        board.IsVotingOpen = false;
        board.IsSuggestionsOpen = false;
        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBoardAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        _boardRepository.Delete(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }
}
