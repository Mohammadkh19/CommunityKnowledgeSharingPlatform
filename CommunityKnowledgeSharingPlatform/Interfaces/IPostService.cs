using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Helpers;
using CommunityKnowledgeSharingPlatform.Models;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IPostService
    {
        Task<(bool Success, string Message)> CreatePostAsync(PostDTO postDto, string userId);

        Task<(bool Success, string Message)> EditPostAsync(PostDTO postDto, string userId);

        Task<(bool Success, string Message)> DeletePostAsync(int postId, string userId);
        Task<PostByIdDTO> GetPostByIdAsync(int postId, string currentUserId);

        PaginatedResponse<FetchingPostDTO> GetPosts(string? search, string? username, int categoryId, string currentUserId, int page = 1, int pageSize = 10);
    }
}
