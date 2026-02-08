using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.DTOs;
using VoterAPI.Models;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Services;

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

    public async Task<BoardDetailDto?> GetBoardWithDetailsAsync(int id, int? currentUserId = null, bool isAdmin = false)
    {
        var board = await _boardRepository.GetBoardWithDetailsAsync(id);
        if (board == null) return null;

        var boardDetail = _mapper.Map<BoardDetailDto>(board);

        // Admins see all suggestions, regular users see visible + their own pending
        var visibleSuggestions = board.Suggestions
            .Where(s => isAdmin || s.IsVisible || 
                (currentUserId.HasValue && s.SubmittedByUserId == currentUserId.Value))
            .ToList();

        boardDetail.Suggestions = visibleSuggestions.Select(s =>
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

        board.Title = dto.Title;
        board.Description = dto.Description;
        board.IsSuggestionsOpen = dto.IsSuggestionsOpen;
        board.IsVotingOpen = dto.IsVotingOpen;
        board.RequireApproval = dto.RequireApproval;
        board.VotingType = Enum.Parse<VotingType>(dto.VotingType);
        board.MaxVotes = dto.MaxVotes;

        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return _mapper.Map<BoardDto>(board);
    }

    public async Task<bool> ToggleVotingStatusAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        board.IsVotingOpen = !board.IsVotingOpen;
        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleSuggestionsStatusAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        board.IsSuggestionsOpen = !board.IsSuggestionsOpen;
        _boardRepository.Update(board);
        await _boardRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleBoardStatusAsync(int id)
    {
        var board = await _boardRepository.GetByIdAsync(id);
        if (board == null) return false;

        board.IsClosed = !board.IsClosed;
        if (board.IsClosed)
        {
            board.IsVotingOpen = false;
            board.IsSuggestionsOpen = false;
        }
        
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
