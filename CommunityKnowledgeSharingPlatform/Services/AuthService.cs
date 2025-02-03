using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<Users> userManager, SignInManager<Users> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Token, string Message)> RegisterAsync(RegisterDTO registerDto)
        {
            var user = new Users
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                // Return error messages if creation failed
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, null, $"User creation failed: {errors}");
            }


            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                // If role assignment fails, delete the user to avoid inconsistent data
                await _userManager.DeleteAsync(user);
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return (false, null, $"Role assignment failed: {errors}");
            }

            var userSession = new UserSessionDTO(user.Id, user.UserName, "USER");
            var token = GenerateJwtToken(userSession);
            return (true, token, "User registered successfully.");
        }

        public async Task<(bool Success, string Token, string Message)> LoginAsync(LoginDTO loginDto)
        {
            // Check if user exists by either email or username
            var user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail) ??
                       await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);

            if (user == null)
                return (false, null, "User not found.");

            // Check if the password is correct
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!passwordCheck.Succeeded)
                return (false, null, "Invalid credentials.");



            // Fetch user roles
            var roles = await _userManager.GetRolesAsync(user);

            var userSession = new UserSessionDTO(user.Id, user.UserName, roles.First());
            var token = GenerateJwtToken(userSession);

            return (true, token, "Login successful.");
        }


        public async Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDto)
        {
            // Validate that NewPassword matches ConfirmNewPassword
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return (false, "New password and confirmation password do not match.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"{errors}");
            }

            return (true, "Password changed successfully.");
        }



        private string GenerateJwtToken(UserSessionDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"!]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        };
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                userClaims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<(bool Success, string UserName, string Email, string Message)> GetUserDetailsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, null, null, "User not found.");
            }

            return (true, user.UserName, user.Email, "User details retrieved successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateUserDetailsAsync(string userId, string newUsername, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            bool isUpdated = false;

            // Update the username if it has changed
            if (!string.IsNullOrEmpty(newUsername) && user.UserName != newUsername)
            {
                var usernameResult = await _userManager.SetUserNameAsync(user, newUsername);
                if (!usernameResult.Succeeded)
                {
                    var errors = string.Join(", ", usernameResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to update username: {errors}");
                }
                isUpdated = true;
            }

            // Update the email if it has changed
            if (!string.IsNullOrEmpty(newEmail) && user.Email != newEmail)
            {
                var emailResult = await _userManager.SetEmailAsync(user, newEmail);
                if (!emailResult.Succeeded)
                {
                    var errors = string.Join(", ", emailResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to update email: {errors}");
                }
                isUpdated = true;
            }

            // If no updates were made, return a message
            if (!isUpdated)
            {
                return (true, "No changes were made.");
            }

            return (true, "User details updated successfully.");
        }


    }
}
