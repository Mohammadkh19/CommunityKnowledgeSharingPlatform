using System;
using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public CommentService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<(bool Success, string Message, object? Data)> AddCommentAsync(CommentDTO commentDto, string userId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(commentDto.PostId);

                if (post == null)
                {
                    return (false, "Post not found", null);
                }

                var user = await _context.Users
                    .Include(u => u.Profile)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return (false, "User not found.", null);
                }


                var comment = new Comments
                {
                    CommentText = commentDto.Content,
                    PostId = commentDto.PostId,
                    UserId = userId,
                    CommentDate = DateTime.UtcNow
                };

                _context.Comments.Add(comment);

                
                var postOwnerId = post.UserId;
                if (postOwnerId != userId)
                {
                    var notificationDto = new NotificationDTO
                    {
                        Content = $"Your post has a new comment",
                        PostId = post.PostId
                    };
                    await _notificationService.AddNotificationAsync(notificationDto, postOwnerId);
                }
                

                await _context.SaveChangesAsync();

                var response = new
                {
                    CommentId = comment.CommentId,
                    Username = user.UserName,
                    CommentText = comment.CommentText,
                    ProfilePicturePath = user.Profile.ProfilePicturePath,
                    CommentDate = comment.CommentDate
                };

                return (true, "Comment added successfully", response);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> EditCommentAsync(CommentDTO commentDto, string userId)
        {
            try
            {
                // Find the comment by ID
                var comment = await _context.Comments.FindAsync(commentDto.CommentId);

                if (comment == null)
                {
                    return (false, "Comment not found");
                }

                // Ensure the user is the owner of the comment
                if (comment.UserId != userId)
                {
                    return (false, "You are not authorized to edit this comment");
                }

                // Update comment properties
                comment.CommentText = commentDto.Content;

                // Save changes
                await _context.SaveChangesAsync();

                return (true, "Comment updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while updating the comment: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCommentAsync(int commentId, string userId)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.Post) // Ensure Post is loaded
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

                if (comment == null)
                {
                    return (false, $"Comment with ID {commentId} not found");
                }

                if (comment.UserId != userId)
                {
                    return (false, "You are not authorized to delete this comment");
                }
                _context.Comments.Remove(comment);

                var postOwnerId = comment.Post.UserId;
                if (postOwnerId != userId)
                {
                    var notificationText = $"Your post has a new comment";
                    var notification = await _context.Notifications
                        .FirstOrDefaultAsync(n =>
                        n.PostId == comment.PostId &&
                        n.UserId == comment.Post.UserId &&
                        n.NotificationText == notificationText);
                    if (notification != null)
                    {
                        _context.Notifications.Remove(notification);
                    }
                }


                await _context.SaveChangesAsync();
                return (true, "Comment deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting the comment: {ex.Message}");
            }
        }


    }
}
