using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class VoteService : IVoteService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public VoteService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<(bool Success, string Message)> VotePostAsync(VoteDTO voteDto , string userId)
        {
            try
            {
                // Check if the post exists
                var post = await _context.Posts.FindAsync(voteDto.PostId);
                if (post == null)
                {
                    return (false, "Post not found");
                }

                // Check if the user has already voted on this post
                var existingVote = await _context.Votes
                    .FirstOrDefaultAsync(v => v.PostId == voteDto.PostId && v.UserId == userId);

                if (existingVote != null)
                {
                    if (userId != post.UserId)
                    {
                        var notification = await _context.Notifications
                            .FirstOrDefaultAsync(n =>
                            n.PostId == post.PostId &&
                            n.UserId == post.UserId
                            );

                        if (notification != null)
                        {
                            _context.Notifications.Remove(notification);
                        }
                    }


                    // If the vote is the same as the current one, remove it (toggle off)
                    if (existingVote.IsUpvote == voteDto.IsUpvote)
                    {
                        _context.Votes.Remove(existingVote);
                        await _context.SaveChangesAsync();

                        return (true, "Vote removed successfully");
                    }

                    // Update the vote if it's different
                    existingVote.IsUpvote = voteDto.IsUpvote; 
                }
                else
                {
                    // Create a new vote
                    var vote = new Votes
                    {
                        UserId = userId,
                        PostId = voteDto.PostId,
                        IsUpvote = voteDto.IsUpvote
                    };
                    _context.Votes.Add(vote);
                }

                if (userId != post.UserId)
                {
                    var newAction = voteDto.IsUpvote ? "upvoted" : "downvoted";
                    var notificationDto = new NotificationDTO
                    {
                        PostId = voteDto.PostId,
                        Content = $"Your post was {newAction} by a user."
                    };
                    var postOwnerId = post.UserId;
                    await _notificationService.AddNotificationAsync(notificationDto, postOwnerId);

                }
                await _context.SaveChangesAsync();
                return (true, "Vote registered successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while voting: {ex.Message}");
            }
        }

    }
}
