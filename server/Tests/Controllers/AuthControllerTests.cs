using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VoterAPI.Controllers;
using VoterAPI.DTOs;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _sut = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Register_Should_ReturnOk_When_RegistrationSucceeds()
    {
        // Arrange
        var registerDto = new RegisterRequestDto
        {
            Username = "newuser",
            Password = "password123"
        };
        var response = new LoginResponseDto
        {
            User = new UserDto
            {
                Id = 1,
                Username = "newuser",
                Role = "User",
                CreatedDate = DateTime.UtcNow
            },
            Token = "fake-jwt-token"
        };
        _mockAuthService.Setup(s => s.RegisterAsync(registerDto)).ReturnsAsync(response);

        // Act
        var result = await _sut.Register(registerDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<LoginResponseDto>().Subject;
        returnedResponse.User.Username.Should().Be("newuser");
        returnedResponse.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_Should_ReturnOk_When_LoginSucceeds()
    {
        // Arrange
        var loginDto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "password123"
        };
        var response = new LoginResponseDto
        {
            User = new UserDto
            {
                Id = 1,
                Username = "testuser",
                Role = "User",
                CreatedDate = DateTime.UtcNow
            },
            Token = "fake-jwt-token"
        };
        _mockAuthService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(response);

        // Act
        var result = await _sut.Login(loginDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<LoginResponseDto>().Subject;
        returnedResponse.User.Username.Should().Be("testuser");
        returnedResponse.Token.Should().NotBeNullOrEmpty();
    }
}
