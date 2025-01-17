namespace CommunityKnowledgeSharingPlatform.Models
{
    public class Notifications
    {
        public int NotificationId { get; set; }
        public string NotificationText { get; set; }
        public bool IsRead { get; set; }
        public DateTime NotificationDate { get; set; }

        public int PostId { get; set; }
        public Posts Post { get; set; }
        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
