using Microsoft.Extensions.Hosting;

namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Comments
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }

        public int PostId { get; set; }
        public Posts Post { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
