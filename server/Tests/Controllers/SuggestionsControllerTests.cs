using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using VoterAPI.Controllers;
using VoterAPI.DTOs;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Tests.Controllers;

public class SuggestionsControllerTests
{
    private readonly Mock<ISuggestionService> _mockSuggestionService;
    private readonly SuggestionsController _sut;

    public SuggestionsControllerTests()
    {
        _mockSuggestionService = new Mock<ISuggestionService>();
        _sut = new SuggestionsController(_mockSuggestionService.Object);
        
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
    public async Task CreateSuggestion_Should_ReturnCreatedSuggestion()
    {
        // Arrange
        var createDto = new SuggestionCreateDto { Text = "New Suggestion" };
        var createdSuggestion = new SuggestionDto
        {
            Id = 1,
            Text = "New Suggestion",
            BoardId = 1,
            Status = "Approved",
            SubmittedByUserId = 1,
            SubmittedByUsername = "user1",
            SubmittedDate = DateTime.UtcNow
        };
        _mockSuggestionService.Setup(s => s.CreateSuggestionAsync(1, createDto, It.IsAny<int>())).ReturnsAsync(createdSuggestion);

        // Act
        var result = await _sut.CreateSuggestion(1, createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedSuggestion = createdResult.Value.Should().BeOfType<SuggestionDto>().Subject;
        returnedSuggestion.Text.Should().Be("New Suggestion");
    }

    [Fact]
    public async Task DeleteSuggestion_Should_ReturnNoContent_When_DeleteSucceeds()
    {
        // Arrange
        _mockSuggestionService.Setup(s => s.DeleteSuggestionAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteSuggestion(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
