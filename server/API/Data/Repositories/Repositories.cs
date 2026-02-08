using Microsoft.EntityFrameworkCore;
using VoterAPI.Models;

namespace VoterAPI.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(VoterDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }
}

public class BoardRepository : Repository<Board>, IBoardRepository
{
    public BoardRepository(VoterDbContext context) : base(context)
    {
    }

    public async Task<Board?> GetBoardWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Suggestions)
                .ThenInclude(s => s.Votes)
            .Include(b => b.Suggestions)
                .ThenInclude(s => s.SubmittedBy)
            .Include(b => b.CreatedBy)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Board>> GetBoardsByUserAsync(int userId)
    {
        return await _dbSet
            .Where(b => b.CreatedByUserId == userId)
            .ToListAsync();
    }
}

public class SuggestionRepository : Repository<Suggestion>, ISuggestionRepository
{
    public SuggestionRepository(VoterDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Suggestion>> GetSuggestionsByBoardIdAsync(int boardId)
    {
        return await _dbSet
            .Include(s => s.Votes)
            .Include(s => s.SubmittedBy)
            .Where(s => s.BoardId == boardId && s.Status == SuggestionStatus.Approved)
            .ToListAsync();
    }

    public async Task<IEnumerable<Suggestion>> GetPendingSuggestionsAsync()
    {
        return await _dbSet
            .Include(s => s.Board)
            .Include(s => s.SubmittedBy)
            .Where(s => s.Status == SuggestionStatus.Pending)
            .OrderBy(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<Suggestion?> GetSuggestionWithVotesAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Votes)
            .Include(s => s.SubmittedBy)
            .Include(s => s.Board)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}

public class VoteRepository : Repository<Vote>, IVoteRepository
{
    public VoteRepository(VoterDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Vote>> GetVotesBySuggestionIdAsync(int suggestionId)
    {
        return await _dbSet
            .Include(v => v.User)
            .Where(v => v.SuggestionId == suggestionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vote>> GetUserVotesByBoardIdAsync(int userId, int boardId)
    {
        return await _dbSet
            .Include(v => v.Suggestion)
            .Where(v => v.UserId == userId && v.Suggestion!.BoardId == boardId)
            .ToListAsync();
    }

    public async Task<Vote?> GetUserVoteOnSuggestionAsync(int userId, int suggestionId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(v => v.UserId == userId && v.SuggestionId == suggestionId);
    }

    public async Task<int> GetVoteCountBySuggestionIdAsync(int suggestionId)
    {
        return await _dbSet.CountAsync(v => v.SuggestionId == suggestionId);
    }
}
