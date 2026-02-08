using FluentAssertions;
using Moq;
using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.Models;
using VoterAPI.Services;
using VoterAPI.DTOs;

namespace VoterAPI.Tests.Services;

public class VoteServiceTests
{
    private readonly Mock<IVoteRepository> _mockVoteRepo;
    private readonly Mock<ISuggestionRepository> _mockSuggestionRepo;
    private readonly Mock<IBoardRepository> _mockBoardRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly VoteService _sut;

    public VoteServiceTests()
    {
        _mockVoteRepo = new Mock<IVoteRepository>();
        _mockSuggestionRepo = new Mock<ISuggestionRepository>();
        _mockBoardRepo = new Mock<IBoardRepository>();
        _mockMapper = new Mock<IMapper>();
        _sut = new VoteService(_mockVoteRepo.Object, _mockSuggestionRepo.Object, _mockBoardRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddVoteAsync_Should_AddVote_WhenValid()
    {
        // Arrange
        var board = new Board
        {
            Id = 1,
            Title = "Test Board",
            Description = "Test",
            IsVotingOpen = true,
            VotingType = VotingType.Single,
            CreatedByUserId = 1,
            CreatedDate = DateTime.UtcNow
        };
        var suggestion = new Suggestion
        {
            Id = 1,
            Text = "Test Suggestion",
            BoardId = 1,
            Board = board,
            SubmittedByUserId = 2,
            SubmittedDate = DateTime.UtcNow,
            Status = SuggestionStatus.Approved,
            IsVisible = true,
            Votes = new List<Vote>()
        };
        var suggestionId = 1;
        var userId = 3;
        var createdVote = new Vote
        {
            Id = 1,
            SuggestionId = suggestionId,
            UserId = userId,
            VotedDate = DateTime.UtcNow
        };
        var voteDto = new VoteDto
        {
            Id = 1,
            SuggestionId = suggestionId,
            UserId = userId,
            Username = "user3",
            VotedDate = DateTime.UtcNow
        };

        _mockSuggestionRepo.Setup(r => r.GetSuggestionWithVotesAsync(suggestionId)).ReturnsAsync(suggestion);
        _mockVoteRepo.Setup(r => r.GetUserVotesByBoardIdAsync(userId, 1)).ReturnsAsync(new List<Vote>());
        _mockVoteRepo.Setup(r => r.AddAsync(It.IsAny<Vote>())).Returns(Task.CompletedTask);
        _mockVoteRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<VoteDto>(It.IsAny<Vote>())).Returns(voteDto);

        // Act
        var result = await _sut.AddVoteAsync(suggestionId, userId);

        // Assert
        result.Should().NotBeNull();
        result.SuggestionId.Should().Be(suggestionId);
    }

    [Fact]
    public async Task RemoveVoteAsync_Should_RemoveVote_When_VoteExists()
    {
        // Arrange
        var vote = new Vote
        {
            Id = 1,
            SuggestionId = 1,
            UserId = 1,
            VotedDate = DateTime.UtcNow
        };
        _mockVoteRepo.Setup(r => r.GetUserVoteOnSuggestionAsync(1, 1)).ReturnsAsync(vote);
        _mockVoteRepo.Setup(r => r.Delete(vote));
        _mockVoteRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.RemoveVoteAsync(1, 1);

        // Assert
        result.Should().BeTrue();
    }
}
