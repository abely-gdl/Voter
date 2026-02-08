using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoterAPI.DTOs;
using VoterAPI.Services;

namespace VoterAPI.Controllers;

[ApiController]
[Route("api/suggestions")]
public class SuggestionsController : ControllerBase
{
    private readonly ISuggestionService _suggestionService;

    public SuggestionsController(ISuggestionService suggestionService)
    {
        _suggestionService = suggestionService;
    }

    /// <summary>
    /// Get pending suggestions (Admin only)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<SuggestionDto>>> GetPendingSuggestions()
    {
        var suggestions = await _suggestionService.GetPendingSuggestionsAsync();
        return Ok(suggestions);
    }

    /// <summary>
    /// Create a suggestion for a board (requires authentication)
    /// </summary>
    [HttpPost("boards/{boardId}")]
    [Authorize]
    public async Task<ActionResult<SuggestionDto>> CreateSuggestion(int boardId, [FromBody] SuggestionCreateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        try
        {
            var suggestion = await _suggestionService.CreateSuggestionAsync(boardId, dto, userId);
            return CreatedAtAction(nameof(GetSuggestion), new { id = suggestion.Id }, suggestion);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific suggestion
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SuggestionDto>> GetSuggestion(int id)
    {
        var suggestion = await _suggestionService.GetSuggestionByIdAsync(id);
        if (suggestion == null)
        {
            return NotFound();
        }

        return Ok(suggestion);
    }

    /// <summary>
    /// Approve a suggestion (Admin only)
    /// </summary>
    [HttpPut("{id}/approve")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> ApproveSuggestion(int id)
    {
        var result = await _suggestionService.ApproveSuggestionAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Reject a suggestion (Admin only)
    /// </summary>
    [HttpPut("{id}/reject")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> RejectSuggestion(int id)
    {
        var result = await _suggestionService.RejectSuggestionAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a suggestion (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteSuggestion(int id)
    {
        var result = await _suggestionService.DeleteSuggestionAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
