using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.DTOs;
using VoterAPI.Models;

namespace VoterAPI.Services;

public interface IVoteService
{
    Task<VoteDto> AddVoteAsync(int suggestionId, int userId);
    Task<bool> RemoveVoteAsync(int suggestionId, int userId);
    Task<IEnumerable<VoteDto>> GetVotesBySuggestionIdAsync(int suggestionId);
    Task<IEnumerable<VoteDto>> GetUserVotesByBoardIdAsync(int userId, int boardId);
}

public class VoteService : IVoteService
{
    private readonly IVoteRepository _voteRepository;
    private readonly ISuggestionRepository _suggestionRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IMapper _mapper;

    public VoteService(
        IVoteRepository voteRepository,
        ISuggestionRepository suggestionRepository,
        IBoardRepository boardRepository,
        IMapper mapper)
    {
        _voteRepository = voteRepository;
        _suggestionRepository = suggestionRepository;
        _boardRepository = boardRepository;
        _mapper = mapper;
    }

    public async Task<VoteDto> AddVoteAsync(int suggestionId, int userId)
    {
        // Get suggestion with board info
        var suggestion = await _suggestionRepository.GetSuggestionWithVotesAsync(suggestionId);
        if (suggestion == null)
        {
            throw new InvalidOperationException("Suggestion not found");
        }

        var board = suggestion.Board;
        if (board == null)
        {
            throw new InvalidOperationException("Board not found");
        }

        // Check if voting is open
        if (!board.IsVotingOpen)
        {
            throw new InvalidOperationException("Voting is not currently open for this board");
        }

        // Check if board is closed
        if (board.IsClosed)
        {
            throw new InvalidOperationException("This board is closed");
        }

        // Check if suggestion is approved
        if (suggestion.Status != SuggestionStatus.Approved)
        {
            throw new InvalidOperationException("Cannot vote on unapproved suggestion");
        }

        // Check if user already voted on this suggestion
        var existingVote = await _voteRepository.GetUserVoteOnSuggestionAsync(userId, suggestionId);
        if (existingVote != null)
        {
            throw new InvalidOperationException("You have already voted for this suggestion");
        }

        // Validate voting rules
        await ValidateVotingRulesAsync(board, userId, suggestionId);

        // Add vote
        var vote = new Vote
        {
            SuggestionId = suggestionId,
            UserId = userId,
            VotedDate = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(vote);
        await _voteRepository.SaveChangesAsync();

        return _mapper.Map<VoteDto>(vote);
    }

    public async Task<bool> RemoveVoteAsync(int suggestionId, int userId)
    {
        var vote = await _voteRepository.GetUserVoteOnSuggestionAsync(userId, suggestionId);
        if (vote == null) return false;

        _voteRepository.Delete(vote);
        await _voteRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<VoteDto>> GetVotesBySuggestionIdAsync(int suggestionId)
    {
        var votes = await _voteRepository.GetVotesBySuggestionIdAsync(suggestionId);
        return _mapper.Map<IEnumerable<VoteDto>>(votes);
    }

    public async Task<IEnumerable<VoteDto>> GetUserVotesByBoardIdAsync(int userId, int boardId)
    {
        var votes = await _voteRepository.GetUserVotesByBoardIdAsync(userId, boardId);
        return _mapper.Map<IEnumerable<VoteDto>>(votes);
    }

    private async Task ValidateVotingRulesAsync(Board board, int userId, int suggestionId)
    {
        var userVotesOnBoard = await _voteRepository.GetUserVotesByBoardIdAsync(userId, board.Id);
        var voteCount = userVotesOnBoard.Count();

        // Single vote validation
        if (board.VotingType == VotingType.Single && voteCount >= 1)
        {
            throw new InvalidOperationException("This board only allows one vote per user");
        }

        // Multiple votes with max validation
        if (board.VotingType == VotingType.Multiple && board.MaxVotes.HasValue)
        {
            if (voteCount >= board.MaxVotes.Value)
            {
                throw new InvalidOperationException($"You have reached the maximum number of votes ({board.MaxVotes.Value}) for this board");
            }
        }
    }
}
