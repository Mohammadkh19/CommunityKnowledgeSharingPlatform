namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Posts
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedAt { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }

        public int CategoryId { get; set; }
        public Categories Category { get; set; }

        // Navigation properties
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Votes> Votes { get; set; }
        public ICollection<Notifications> Notifications { get; set; }
    }
}
