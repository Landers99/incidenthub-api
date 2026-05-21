using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using IncidentHub.Api.Services;
using IncidentHub.Api.DTOs.Comments;

using System.Security.Claims;

namespace IncidentHub.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost("incidents/{incidentId:guid}/comments")]
    public async Task<ActionResult<CommentResponse>> AddComment(Guid incidentId, CreateCommentRequest request)
    {
        var userId = GetCurrentUserId();

        var response = await _commentService.AddCommentAsync(incidentId, userId, request);

        return CreatedAtAction(nameof(GetCommentsForIncident), new { incidentId }, response);
    }

    [HttpGet("incidents/{incidentId:guid}/comments")]
    public async Task<ActionResult<List<CommentResponse>>> GetCommentsForIncident(Guid incidentId)
    {
        var comments = await _commentService.GetCommentsForIncidentAsync(incidentId);

        return Ok(comments);
    }

    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        await _commentService.DeleteCommentAsync(commentId, userId, isAdmin);

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = 
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub") ??
            User.FindFirstValue("userId");

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            throw new UnauthorizedAccessException("Missing user ID claim.");
        }

        return Guid.Parse(userIdClaim);
    }
}
