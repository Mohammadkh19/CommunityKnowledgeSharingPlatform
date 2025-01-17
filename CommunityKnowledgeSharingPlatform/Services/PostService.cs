using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class PostService: IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CreatePostAsync(PostDTO postDto, string userId)
        {
            try
            {
                var post = new Posts
                {
                    Title = postDto.PostTitle,
                    Description = postDto.PostDescription,
                    CategoryId = postDto.CategoryId,
                    UserId = userId,
                    PostedAt = DateTime.UtcNow,
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return (true, "Post Created successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while adding the post: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> EditPostAsync(PostDTO postDto, string userId)
        {
            try
            {
                // Find the post by ID
                var post = await _context.Posts.FindAsync(postDto.PostId);

                if (post == null)
                {
                    return (false, "Post not found");
                }

                // Ensure the user is the owner of the post
                if (post.UserId != userId)
                {
                    return (false, "You are not authorized to edit this post");
                }

                // Update post properties
                post.Title = postDto.PostTitle;
                post.Description = postDto.PostDescription;
                post.CategoryId = postDto.CategoryId;

                // Save changes
                await _context.SaveChangesAsync();

                return (true, "Post updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while updating the post: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeletePostAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return (false, $"Post with ID {postId} not found");
                }

                if (post.UserId != userId)
                {
                    return (false, "You are not authorized to delete this post");
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return (true, "Post deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting the post: {ex.Message}");
            }
        }

    }
}
