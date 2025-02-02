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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("createPost")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] PostDTO postDto)
        {
            // Get the UserId of the current authorized user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _postService.CreatePostAsync(postDto, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPut("editPost")]
        [Authorize]
        public async Task<IActionResult> EditPost([FromBody] PostDTO postDto)
        {
            // Get the UserId of the current authorized user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var (success, message) = await _postService.EditPostAsync(postDto, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [Authorize]
        [HttpDelete("deletePost/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _postService.DeletePostAsync(id, userId);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpGet("getPosts")]
        public IActionResult GetPosts([FromQuery] string? search, [FromQuery] string? username, [FromQuery] int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID

            var response = _postService.GetPosts(search, username, categoryId, currentUserId, page, pageSize);

            return Ok(response); // Return the paginated response
        }

        [Authorize]
        [HttpGet("getPosts/{postId}")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID


            var post = await _postService.GetPostByIdAsync(postId, currentUserId);

            if (post == null)
            {
                return NotFound(new { Message = $"Post with ID {postId} was not found." });
            }

            return Ok(post);
      
        }
    }
}
