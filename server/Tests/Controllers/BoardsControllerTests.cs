using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using VoterAPI.Controllers;
using VoterAPI.DTOs;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Tests.Controllers;

public class BoardsControllerTests
{
    private readonly Mock<IBoardService> _mockBoardService;
    private readonly BoardsController _sut;

    public BoardsControllerTests()
    {
        _mockBoardService = new Mock<IBoardService>();
        _sut = new BoardsController(_mockBoardService.Object);
        
        // Setup mock user for authenticated endpoints
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetAllBoards_Should_ReturnOkWithBoards()
    {
        // Arrange
        var boards = new List<BoardDto>
        {
            new BoardDto { Id = 1, Title = "Board 1", Description = "Desc 1", VotingType = "Single", SuggestionCount = 0, TotalVotes = 0 },
            new BoardDto { Id = 2, Title = "Board 2", Description = "Desc 2", VotingType = "Multiple", SuggestionCount = 0, TotalVotes = 0 }
        };
        _mockBoardService.Setup(s => s.GetAllBoardsAsync()).ReturnsAsync(boards);

        // Act
        var result = await _sut.GetAllBoards();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBoards = okResult.Value.Should().BeAssignableTo<IEnumerable<BoardDto>>().Subject;
        returnedBoards.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBoardDetails_Should_ReturnOk_When_BoardExists()
    {
        // Arrange
        var boardDetail = new BoardDetailDto
        {
            Id = 1,
            Title = "Test Board",
            Description = "Description",
            VotingType = "Single",
            Suggestions = new List<SuggestionWithVotesDto>()
        };
        _mockBoardService.Setup(s => s.GetBoardWithDetailsAsync(1, It.IsAny<int?>(), It.IsAny<bool>())).ReturnsAsync(boardDetail);

        // Act
        var result = await _sut.GetBoardDetails(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBoard = okResult.Value.Should().BeOfType<BoardDetailDto>().Subject;
        returnedBoard.Title.Should().Be("Test Board");
    }

    [Fact]
    public async Task CreateBoard_Should_ReturnCreatedBoard()
    {
        // Arrange
        var createDto = new BoardCreateDto
        {
            Title = "New Board",
            Description = "New Description",
            RequireApproval = false,
            VotingType = "Single"
        };
        var createdBoard = new BoardDto
        {
            Id = 1,
            Title = "New Board",
            Description = "New Description",
            VotingType = "Single",
            SuggestionCount = 0,
            TotalVotes = 0
        };
        _mockBoardService.Setup(s => s.CreateBoardAsync(It.IsAny<BoardCreateDto>(), It.IsAny<int>())).ReturnsAsync(createdBoard);

        // Act
        var result = await _sut.CreateBoard(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedBoard = createdResult.Value.Should().BeOfType<BoardDto>().Subject;
        returnedBoard.Title.Should().Be("New Board");
    }
}
