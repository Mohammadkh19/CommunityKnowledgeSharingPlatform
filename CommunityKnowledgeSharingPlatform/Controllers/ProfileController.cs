using CommunityKnowledgeSharingPlatform.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CommunityKnowledgeSharingPlatform.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CommunityKnowledgeSharingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }
        [Authorize]
        [HttpPost("AddProfile")]
        public async Task<IActionResult> AddProfile([FromForm] ProfileDTO profileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID
            var result = await _profileService.AddProfileInfoAsync(profileDto, userId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileDTO profileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var result = await _profileService.UpdateProfileInfoAsync(profileDto, userId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [Authorize]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile(string? username = null)
        {

            username ??= User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid username.");
            }

            var profile = await _profileService.GetProfileByUserAsync(username);
            if (profile == null)
            {
                return Ok(null); // No profile exists
            }

            return Ok(profile); // Return existing profile
        }

    }
}
