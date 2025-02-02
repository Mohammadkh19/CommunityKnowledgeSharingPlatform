using CommunityKnowledgeSharingPlatform.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Services;

namespace CommunityKnowledgeSharingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("addComment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentDTO commentDto)
        {
            // Get the UserId of the current authorized user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message, data) = await _commentService.AddCommentAsync(commentDto, userId);

            if (!success)
            {
                return BadRequest(new { Message = message });
            }
            return Ok(new { Message = message, Comment = data });
        }

        [HttpPut("editComment")]
        [Authorize]
        public async Task<IActionResult> EditComment([FromBody] CommentDTO commentDto)
        {
            // Get the UserId of the current authorized user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _commentService.EditCommentAsync(commentDto, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [Authorize]
        [HttpDelete("deleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _commentService.DeleteCommentAsync(id, userId);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }
    }
}
