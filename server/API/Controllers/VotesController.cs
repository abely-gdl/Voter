using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoterAPI.DTOs;
using VoterAPI.Services.Interfaces;

namespace VoterAPI.Controllers;

[ApiController]
[Route("api/votes")]
[Authorize]
public class VotesController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VotesController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    /// <summary>
    /// Vote on a suggestion (requires authentication)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VoteDto>> Vote([FromBody] VoteCreateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        try
        {
            var vote = await _voteService.AddVoteAsync(dto.SuggestionId, userId);
            return CreatedAtAction(nameof(GetVotesBySuggestion), new { suggestionId = dto.SuggestionId }, vote);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove vote from a suggestion (requires authentication)
    /// </summary>
    [HttpDelete("{suggestionId}")]
    public async Task<ActionResult> RemoveVote(int suggestionId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var result = await _voteService.RemoveVoteAsync(suggestionId, userId);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Get all votes for a suggestion
    /// </summary>
    [HttpGet("suggestion/{suggestionId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<VoteDto>>> GetVotesBySuggestion(int suggestionId)
    {
        var votes = await _voteService.GetVotesBySuggestionIdAsync(suggestionId);
        return Ok(votes);
    }

    /// <summary>
    /// Get current user's votes for a board
    /// </summary>
    [HttpGet("board/{boardId}/my-votes")]
    public async Task<ActionResult<IEnumerable<VoteDto>>> GetMyVotesForBoard(int boardId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var votes = await _voteService.GetUserVotesByBoardIdAsync(userId, boardId);
        return Ok(votes);
    }
}
