namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class FetchingPostDTO
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PostedBy { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public string Category { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime PostedAt { get; set; }
        public string PostedAtRelative { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public bool IsMyPost { get; set; }
        public bool IsCurrentUserComment { get; set; }
    }
}
