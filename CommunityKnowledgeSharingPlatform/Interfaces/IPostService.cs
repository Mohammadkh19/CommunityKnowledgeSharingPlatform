using CommunityKnowledgeSharingPlatform.DTOs;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IPostService
    {
        Task<(bool Success, string Message)> CreatePostAsync(PostDTO postDto, string userId);

        Task<(bool Success, string Message)> EditPostAsync(PostDTO postDto, string userId);

        Task<(bool Success, string Message)> DeletePostAsync(int postId, string userId);

    }
}
