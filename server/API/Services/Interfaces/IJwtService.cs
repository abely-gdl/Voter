using System.Security.Claims;
using VoterAPI.Models;

namespace VoterAPI.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}
