using CommunityKnowledgeSharingPlatform.DTOs;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface ICommentService
    {
        Task<(bool Success, string Message, object? Data)> AddCommentAsync(CommentDTO commentDto, string userId);

        Task<(bool Success, string Message)> EditCommentAsync(CommentDTO commentDto, string userId);
        Task<(bool Success, string Message)> DeleteCommentAsync(int commentId, string userId);
    }
}
