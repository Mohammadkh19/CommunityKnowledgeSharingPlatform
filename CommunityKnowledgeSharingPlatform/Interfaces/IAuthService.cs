using CommunityKnowledgeSharingPlatform.DTOs;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string Message)> RegisterAsync(RegisterDTO registerDto);
        Task<(bool Success, string Token, string Message)> LoginAsync(LoginDTO loginDto);
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDto);
        Task<(bool Success, string UserName, string Email, string Message)> GetUserDetailsAsync(string userId);
        Task<(bool Success, string Message)> UpdateUserDetailsAsync(string userId, string newUsername, string newEmail);
    }
}
