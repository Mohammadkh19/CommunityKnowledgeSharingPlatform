using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Models;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IProfileService
    {
        Task<(bool Success, string Message)> AddProfileInfoAsync(ProfileDTO profileDto, string userId);
        Task<(bool Success, string Message)> UpdateProfileInfoAsync(ProfileDTO profileDto, string userId);
        Task<Profiles> GetProfileByUserAsync(string username);

    }
}
