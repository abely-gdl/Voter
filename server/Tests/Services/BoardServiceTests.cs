using FluentAssertions;
using Moq;
using AutoMapper;
using VoterAPI.Data.Repositories;
using VoterAPI.Models;
using VoterAPI.Services;
using VoterAPI.DTOs;

namespace VoterAPI.Tests.Services;

public class BoardServiceTests
{
    private readonly Mock<IBoardRepository> _mockBoardRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BoardService _sut;

    public BoardServiceTests()
    {
        _mockBoardRepo = new Mock<IBoardRepository>();
        _mockMapper = new Mock<IMapper>();
        _sut = new BoardService(_mockBoardRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllBoardsAsync_Should_ReturnAllBoards()
    {
        // Arrange
        var boards = new List<Board>
        {
            new Board { Id = 1, Title = "Board 1", Description = "Desc 1", CreatedByUserId = 1, CreatedDate = DateTime.UtcNow, VotingType = VotingType.Single },
            new Board { Id = 2, Title = "Board 2", Description = "Desc 2", CreatedByUserId = 1, CreatedDate = DateTime.UtcNow, VotingType = VotingType.Multiple }
        };
        var boardDtos = new List<BoardDto>
        {
            new BoardDto { Id = 1, Title = "Board 1", Description = "Desc 1", VotingType = "Single", SuggestionCount = 0, TotalVotes = 0 },
            new BoardDto { Id = 2, Title = "Board 2", Description = "Desc 2", VotingType = "Multiple", SuggestionCount = 0, TotalVotes = 0 }
        };
        _mockBoardRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(boards);
        _mockMapper.Setup(m => m.Map<IEnumerable<BoardDto>>(boards)).Returns(boardDtos);

        // Act
        var result = await _sut.GetAllBoardsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Board 1");
    }

    [Fact]
    public async Task GetBoardByIdAsync_Should_ReturnBoard_When_BoardExists()
    {
        // Arrlange
        var board = new Board
        {
            Id = 1,
            Title = "Test Board",
            Description = "Description",
            CreatedByUserId = 1,
            CreatedBy = new User { Id = 1, Username = "admin", PasswordHash = "hash", Role = UserRole.Admin, CreatedDate = DateTime.UtcNow },
            CreatedDate = DateTime.UtcNow,
            VotingType = VotingType.Single,
            IsVotingOpen = true,
            IsSuggestionsOpen = true
        };
        var boardDto = new BoardDto
        {
            Id = 1,
            Title = "Test Board",
            Description = "Description",
            VotingType = "Single",
            SuggestionCount = 0,
            TotalVotes = 0
        };
        _mockBoardRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<BoardDto>(board)).Returns(boardDto);

        // Act
        var result = await _sut.GetBoardByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Board");
    }

    [Fact]
    public async Task CreateBoardAsync_Should_CreateNewBoard()
    {
        // Arrange
        var createDto = new BoardCreateDto
        {
            Title = "New Board",
            Description = "New Description",
            RequireApproval = false,
            VotingType = "Single"
        };
        var userId = 1;
        var createdBoard = new Board
        {
            Id = 1,
            Title = "New Board",
            Description = "New Description",
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow,
            VotingType = VotingType.Single
        };
        var boardDto = new BoardDto
        {
            Id = 1,
            Title = "New Board",
            Description = "New Description",
            VotingType = "Single",
            SuggestionCount = 0,
            TotalVotes = 0
        };
        _mockMapper.Setup(m => m.Map<Board>(createDto)).Returns(createdBoard);
        _mockBoardRepo.Setup(r => r.AddAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);
        _mockBoardRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<BoardDto>(It.IsAny<Board>())).Returns(boardDto);

        // Act
        var result = await _sut.CreateBoardAsync(createDto, userId);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("New Board");
    }
}
