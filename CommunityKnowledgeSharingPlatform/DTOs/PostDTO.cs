namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class PostDTO
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }

        public string PostDescription { get; set; }

        public int CategoryId { get; set; }
    }
}
