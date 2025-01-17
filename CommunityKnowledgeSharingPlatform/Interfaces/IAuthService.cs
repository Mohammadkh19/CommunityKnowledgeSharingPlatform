using CommunityKnowledgeSharingPlatform.DTOs;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string Message)> RegisterAsync(RegisterDTO registerDto);
        Task<(bool Success, string Token, string Message)> LoginAsync(LoginDTO loginDto);
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDto);
    }
}
