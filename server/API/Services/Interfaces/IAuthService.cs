using VoterAPI.DTOs;

namespace VoterAPI.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request);
    Task<UserDto?> GetCurrentUserAsync(int userId);
}
