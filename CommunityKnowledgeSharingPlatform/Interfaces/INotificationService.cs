using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Models;

namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(NotificationDTO notificationDto, string userId);
        Task<NotificationResponseDTO> GetNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
    }
}
