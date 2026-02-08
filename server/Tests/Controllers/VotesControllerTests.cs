using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using VoterAPI.Controllers;
using VoterAPI.DTOs;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Tests.Controllers;

public class VotesControllerTests
{
    private readonly Mock<IVoteService> _mockVoteService;
    private readonly VotesController _sut;

    public VotesControllerTests()
    {
        _mockVoteService = new Mock<IVoteService>();
        _sut = new VotesController(_mockVoteService.Object);
        
        // Setup mock user for authenticated endpoints
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Vote_Should_ReturnCreatedVote()
    {
        // Arrange
        var createDto = new VoteCreateDto { SuggestionId = 1 };
        var voteDto = new VoteDto
        {
            Id = 1,
            SuggestionId = 1,
            UserId = 1,
            Username = "user1",
            VotedDate = DateTime.UtcNow
        };
        _mockVoteService.Setup(s => s.AddVoteAsync(1, It.IsAny<int>())).ReturnsAsync(voteDto);

        // Act
        var result = await _sut.Vote(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedVote = createdResult.Value.Should().BeOfType<VoteDto>().Subject;
        returnedVote.SuggestionId.Should().Be(1);
    }

    [Fact]
    public async Task RemoveVote_Should_ReturnNoContent_When_RemoveSucceeds()
    {
        // Arrange
        _mockVoteService.Setup(s => s.RemoveVoteAsync(1, It.IsAny<int>())).ReturnsAsync(true);

        // Act
        var result = await _sut.RemoveVote(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
