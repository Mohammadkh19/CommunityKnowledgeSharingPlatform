namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class PostByIdDTO
    {
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public int CategoryId { get; set; } 
        public string CategoryName { get; set; }
        public DateTime PostedAt { get; set; }
        public string ProfilePicturePath { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public bool IsMyPost { get; set; }
        public List<CommentFetchDTO> Comments { get; set; }
    }
}
