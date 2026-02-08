using VoterAPI.Models;

namespace VoterAPI.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
}

public interface IBoardRepository : IRepository<Board>
{
    Task<Board?> GetBoardWithDetailsAsync(int id);
    Task<IEnumerable<Board>> GetBoardsByUserAsync(int userId);
}

public interface ISuggestionRepository : IRepository<Suggestion>
{
    Task<IEnumerable<Suggestion>> GetSuggestionsByBoardIdAsync(int boardId);
    Task<IEnumerable<Suggestion>> GetPendingSuggestionsAsync();
    Task<Suggestion?> GetSuggestionWithVotesAsync(int id);
}

public interface IVoteRepository : IRepository<Vote>
{
    Task<IEnumerable<Vote>> GetVotesBySuggestionIdAsync(int suggestionId);
    Task<IEnumerable<Vote>> GetUserVotesByBoardIdAsync(int userId, int boardId);
    Task<Vote?> GetUserVoteOnSuggestionAsync(int userId, int suggestionId);
    Task<int> GetVoteCountBySuggestionIdAsync(int suggestionId);
}
