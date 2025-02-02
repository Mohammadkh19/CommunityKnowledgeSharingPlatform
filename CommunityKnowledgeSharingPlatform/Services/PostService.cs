using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Helpers;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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
                var sanitizer = new HtmlSanitizer();

                // Sanitize the description
                string sanitizedDescription = sanitizer.Sanitize(postDto.PostDescription);

                var post = new Posts
                {
                    Title = postDto.PostTitle,
                    Description = sanitizedDescription,
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


        public async Task<PostByIdDTO?> GetPostByIdAsync(int postId, string currentUserId)
        {
            var post = await _context.Posts
                .Where(p => p.PostId == postId)
                .Select(p => new PostByIdDTO
                {
                    PostTitle = p.Title,
                    PostDescription = p.Description,
                    CategoryName = p.Category.CategoryName,
                    CategoryId = p.Category.CategoryId,
                    PostedAt = p.PostedAt,
                    ProfilePicturePath = p.User.Profile.ProfilePicturePath,
                    UpvoteCount = p.Votes.Count(v => v.IsUpvote),
                    DownvoteCount = p.Votes.Count(v => !v.IsUpvote),
                    IsLiked = p.Votes.Any(v => v.IsUpvote && v.UserId == currentUserId),
                    IsDisliked = p.Votes.Any(v => !v.IsUpvote && v.UserId == currentUserId),
                    IsMyPost = p.UserId == currentUserId,
                    Comments = p.Comments.Select(c => new CommentFetchDTO
                    {
                        commentId = c.CommentId,
                        Username = c.User.UserName,
                        Text = c.CommentText,
                        ProfilePicturePath = c.User.Profile.ProfilePicturePath,
                        IsCurrentUserComment = c.UserId == currentUserId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return post; // Will return null if no post is found
        }



        public PaginatedResponse<FetchingPostDTO> GetPosts(string? search, string? username,int categoryId, string currentUserId, int page = 1, int pageSize = 10)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            }

            if (categoryId != 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(p => p.User.UserName == username);
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(p => p.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new FetchingPostDTO
                {
                    PostId = p.PostId,
                    PostTitle = p.Title,
                    PostDescription = p.Description,
                    PostedAt = p.PostedAt,
                    PostedAtRelative = GetRelativeTime(p.PostedAt),
                    Category = p.Category.CategoryName,
                    PostedBy = p.User.UserName,
                    ProfilePicture = p.User.Profile.ProfilePicturePath != null ? p.User.Profile.ProfilePicturePath : null,
                    UpvoteCount = p.Votes.Count(v => v.IsUpvote),
                    DownvoteCount = p.Votes.Count(v => !v.IsUpvote),
                    IsLiked = p.Votes.Any(v => v.IsUpvote && v.UserId == currentUserId),
                    IsDisliked = p.Votes.Any(v => !v.IsUpvote && v.UserId == currentUserId),
                    CommentsCount = p.Comments.Count,
                    IsMyPost = p.UserId == currentUserId,
                    UserId = p.UserId,
                    UserName = p.User.UserName
                }).ToList();

            return new PaginatedResponse<FetchingPostDTO>
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }


        public static string GetRelativeTime(DateTime postedAt)
        {
            var timeSpan = DateTime.UtcNow - postedAt;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} months ago";

            return $"{(int)(timeSpan.TotalDays / 365)} years ago";
        }

    }
}
