using Microsoft.Extensions.Hosting;

namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Votes
    {
        public int VoteId { get; set; }
        public bool IsUpvote { get; set; }

        public int PostId { get; set; }
        public Posts Post { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
