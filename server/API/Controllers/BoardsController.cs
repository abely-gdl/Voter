using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoterAPI.DTOs;
using VoterAPI.Services;

namespace VoterAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardsController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    /// <summary>
    /// Get all boards
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardDto>>> GetAllBoards()
    {
        var boards = await _boardService.GetAllBoardsAsync();
        return Ok(boards);
    }

    /// <summary>
    /// Get board details with suggestions and votes
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BoardDetailDto>> GetBoardDetails(int id)
    {
        // Try to get current user ID if authenticated
        int? currentUserId = null;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
        {
            currentUserId = userId;
        }

        var board = await _boardService.GetBoardWithDetailsAsync(id, currentUserId);
        if (board == null)
        {
            return NotFound();
        }

        return Ok(board);
    }

    /// <summary>
    /// Create a new board (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BoardDto>> CreateBoard([FromBody] BoardCreateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var board = await _boardService.CreateBoardAsync(dto, userId);
        return CreatedAtAction(nameof(GetBoardDetails), new { id = board.Id }, board);
    }

    /// <summary>
    /// Update board settings (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BoardDto>> UpdateBoard(int id, [FromBody] BoardUpdateDto dto)
    {
        var board = await _boardService.UpdateBoardAsync(id, dto);
        if (board == null)
        {
            return NotFound();
        }

        return Ok(board);
    }

    /// <summary>
    /// Toggle voting status for a board (Admin only)
    /// </summary>
    [HttpPut("{id}/toggle-voting")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> ToggleVoting(int id)
    {
        var result = await _boardService.ToggleVotingAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Toggle suggestions status for a board (Admin only)
    /// </summary>
    [HttpPut("{id}/toggle-suggestions")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> ToggleSuggestions(int id)
    {
        var result = await _boardService.ToggleSuggestionsAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Close a board (Admin only)
    /// </summary>
    [HttpPut("{id}/close")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> CloseBoard(int id)
    {
        var result = await _boardService.CloseBoardAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a board (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteBoard(int id)
    {
        var result = await _boardService.DeleteBoardAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
