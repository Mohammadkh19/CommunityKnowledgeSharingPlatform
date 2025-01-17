using CommunityKnowledgeSharingPlatform.DTOs;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IVoteService
    {
        Task<(bool Success, string Message)> VotePostAsync(VoteDTO voteDto , string userId);
    }
}
