namespace CommunityKnowledgeSharingPlatform.DTOs
{
    public class NotificationResponseDTO
    {
        public bool HasUnread { get; set; } // Indicates if at least one notification is unread
        public List<NotificationDTO> Notifications { get; set; }
    }
}
