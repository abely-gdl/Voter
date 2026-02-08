using FluentAssertions;
using Moq;
using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.Models;
using VoterAPI.Services;
using VoterAPI.DTOs;

namespace VoterAPI.Tests.Services;

public class SuggestionServiceTests
{
    private readonly Mock<ISuggestionRepository> _mockSuggestionRepo;
    private readonly Mock<IBoardRepository> _mockBoardRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly SuggestionService _sut;

    public SuggestionServiceTests()
    {
        _mockSuggestionRepo = new Mock<ISuggestionRepository>();
        _mockBoardRepo = new Mock<IBoardRepository>();
        _mockMapper = new Mock<IMapper>();
        _sut = new SuggestionService(_mockSuggestionRepo.Object, _mockBoardRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetSuggestionByIdAsync_Should_ReturnSuggestion_When_SuggestionExists()
    {
        // Arrange
        var suggestion = new Suggestion
        {
            Id = 1,
            Text = "Test Suggestion",
            BoardId = 1,
            SubmittedByUserId = 1,
            SubmittedBy = new User { Id = 1, Username = "user1", PasswordHash = "hash", Role = UserRole.User, CreatedDate = DateTime.UtcNow },
            SubmittedDate = DateTime.UtcNow,
            Status = SuggestionStatus.Pending,
            IsVisible = true,
            Votes = new List<Vote>()
        };
        var suggestionDto = new SuggestionDto
        {
            Id = 1,
            Text = "Test Suggestion",
            BoardId = 1,
            Status = "Pending",
            SubmittedByUserId = 1,
            SubmittedByUsername = "user1",
            SubmittedDate = DateTime.UtcNow
        };
        _mockSuggestionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(suggestion);
        _mockMapper.Setup(m => m.Map<SuggestionDto>(suggestion)).Returns(suggestionDto);

        // Act
        var result = await _sut.GetSuggestionByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("Test Suggestion");
    }

    [Fact]
    public async Task CreateSuggestionAsync_Should_CreateSuggestion()
    {
        // Arrange
        var board = new Board
        {
            Id = 1,
            Title = "Test Board",
            Description = "Test",
            IsSuggestionsOpen = true,
            RequireApproval = false,
            VotingType = VotingType.Single,
            CreatedByUserId = 1,
            CreatedDate = DateTime.UtcNow
        };
        var createDto = new SuggestionCreateDto { Text = "New Suggestion" };
        var userId = 1;
        var createdSuggestion = new Suggestion
        {
            Id = 1,
            Text = "New Suggestion",
            BoardId = 1,
            SubmittedByUserId = userId,
            SubmittedDate = DateTime.UtcNow,
            Status = SuggestionStatus.Approved,
            IsVisible = true
        };
        var suggestionDto = new SuggestionDto
        {
            Id = 1,
            Text = "New Suggestion",
            BoardId = 1,
            Status = "Approved",
            SubmittedByUserId = userId,
            SubmittedByUsername = "user1",
            SubmittedDate = DateTime.UtcNow
        };

        _mockBoardRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<Suggestion>(createDto)).Returns(createdSuggestion);
        _mockSuggestionRepo.Setup(r => r.AddAsync(It.IsAny<Suggestion>())).Returns(Task.CompletedTask);
        _mockSuggestionRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<SuggestionDto>(It.IsAny<Suggestion>())).Returns(suggestionDto);

        // Act
        var result = await _sut.CreateSuggestionAsync(1, createDto, userId);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("New Suggestion");
    }
}
