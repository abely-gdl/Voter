using FluentAssertions;
using Moq;
using VoterAPI.Data.Repositories;
using VoterAPI.Models;
using VoterAPI.Services;
using VoterAPI.Services.Interfaces;
using VoterAPI.DTOs;

namespace VoterAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _sut = new AuthService(_mockUserRepo.Object, _mockJwtService.Object);
    }

    [Fact]
    public async Task RegisterAsync_Should_CreateUser_When_UsernameIsAvailable()
    {
        // Arrange
        var registerDto = new RegisterRequestDto
        {
            Username = "newuser",
            Password = "password123"
        };
        var createdUser = new User
        {
            Id = 1,
            Username = "newuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = UserRole.User,
            CreatedDate = DateTime.UtcNow
        };
        var userDto = new UserDto
        {
            Id = 1,
            Username = "newuser",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };
        _mockUserRepo.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((User?)null);
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _mockUserRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockJwtService.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("fake-jwt-token");

        // Act
        var result = await _sut.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
        result.User.Username.Should().Be("newuser");
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_Should_ReturnToken_When_CredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "password123"
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = passwordHash,
            Role = UserRole.User,
            CreatedDate = DateTime.UtcNow
        };
        var userDto = new UserDto
        {
            Id = 1,
            Username = "testuser",
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };
        _mockUserRepo.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);
        _mockJwtService.Setup(j => j.GenerateToken(user)).Returns("fake-jwt-token");

        // Act
        var result = await _sut.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result!.User.Should().NotBeNull();
        result.User.Username.Should().Be("testuser");
        result.Token.Should().NotBeNullOrEmpty();
    }
}
