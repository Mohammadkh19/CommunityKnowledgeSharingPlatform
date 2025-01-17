using CommunityKnowledgeSharingPlatform.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommunityKnowledgeSharingPlatform.Interfaces;

namespace CommunityKnowledgeSharingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VoteController(IVoteService voteService)
        {
            _voteService = voteService;
        }


        [HttpPost("vote")]
        [Authorize]
        public async Task<IActionResult> VotePost([FromBody] VoteDTO voteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _voteService.VotePostAsync(voteDto, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

    }
}
