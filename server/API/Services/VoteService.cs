using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.DTOs;
using VoterAPI.Models;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Services;

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
        var suggestion = await _suggestionRepository.GetSuggestionWithVotesAsync(suggestionId);
        if (suggestion == null)
        {
            throw new InvalidOperationException("Suggestion not found");
        }

        var board = suggestion.Board ?? throw new InvalidOperationException("Board not found");

        ValidateSuggestionVotingEligibility(suggestion, board);
        await ValidateUserVotingEligibilityAsync(userId, suggestionId, board);

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

    private static void ValidateSuggestionVotingEligibility(Suggestion suggestion, Board board)
    {
        if (!board.IsVotingOpen)
        {
            throw new InvalidOperationException("Voting is not currently open for this board");
        }

        if (board.IsClosed)
        {
            throw new InvalidOperationException("This board is closed");
        }

        if (suggestion.Status != SuggestionStatus.Approved)
        {
            throw new InvalidOperationException("Cannot vote on unapproved suggestion");
        }
    }

    private async Task ValidateUserVotingEligibilityAsync(int userId, int suggestionId, Board board)
    {
        var existingVote = await _voteRepository.GetUserVoteOnSuggestionAsync(userId, suggestionId);
        if (existingVote != null)
        {
            throw new InvalidOperationException("You have already voted for this suggestion");
        }

        await ValidateVotingRulesAsync(board, userId);
    }

    private async Task ValidateVotingRulesAsync(Board board, int userId)
    {
        var userVotesOnBoard = await _voteRepository.GetUserVotesByBoardIdAsync(userId, board.Id);
        var voteCount = userVotesOnBoard.Count();

        if (board.VotingType == VotingType.Single && voteCount >= 1)
        {
            throw new InvalidOperationException("This board only allows one vote per user");
        }

        if (board.VotingType == VotingType.Multiple && board.MaxVotes.HasValue && voteCount >= board.MaxVotes.Value)
        {
            throw new InvalidOperationException($"You have reached the maximum number of votes ({board.MaxVotes.Value}) for this board");
        }
    }
}
