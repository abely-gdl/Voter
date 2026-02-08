using Microsoft.AspNetCore.Mvc;
using VoterAPI.Utils;

namespace VoterAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("hash/{password}")]
    public IActionResult GenerateHash(string password)
    {
        var hash = PasswordHasher.HashPassword(password);
        return Ok(new { password, hash });
    }
}
