namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class CommentFetchDTO
    {
        public int commentId {  get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public string ProfilePicturePath { get; set; }
        public bool IsCurrentUserComment { get; set; }
    }
}
