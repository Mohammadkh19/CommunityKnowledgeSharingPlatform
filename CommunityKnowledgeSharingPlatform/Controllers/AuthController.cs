using System.Security.Claims;
using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunityKnowledgeSharingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var (success, token, message) = await _authService.RegisterAsync(registerDto);

            if (success)
            {
                return Ok(new
                {
                    Message = message,
                    Token = token
                });
            }

            return BadRequest(new { Message = message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var (success, token, message) = await _authService.LoginAsync(loginDto);

            if (success)
            {
                return Ok(new
                {
                    Message = message,                
                    Token = token
                });
            }

            return Unauthorized(new { Message = message });
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (success, message) = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        [Authorize]
        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetDetails()
        {
            // Retrieve the current user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid user token.");
            }

            // Call the service to get user details
            var result = await _authService.GetUserDetailsAsync(userId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new
            {
                Username = result.UserName,
                Email = result.Email
            });
        }

        // Update user details
        [Authorize]
        [HttpPut("UpdateDetails")]
        public async Task<IActionResult> UpdateDetails([FromBody] UpdateUserDTO updateUserDto)
        {
            // Validate input
            if (updateUserDto == null || (string.IsNullOrEmpty(updateUserDto.NewUsername) && string.IsNullOrEmpty(updateUserDto.NewEmail)))
            {
                return BadRequest("Invalid input data.");
            }

            // Retrieve the current user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid user token.");
            }

            // Call the service to update user details
            var result = await _authService.UpdateUserDetailsAsync(userId, updateUserDto.NewUsername, updateUserDto.NewEmail);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

    }
}
