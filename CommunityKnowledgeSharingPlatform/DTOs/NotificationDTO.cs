namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool HasUnread { get; set; }
    }
}
