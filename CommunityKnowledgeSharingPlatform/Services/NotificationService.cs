using CommunityKnowledgeSharingPlatform.DTOs;
using CommunityKnowledgeSharingPlatform.Interfaces;
using CommunityKnowledgeSharingPlatform.Models;
using DiscordRPC;
using Microsoft.EntityFrameworkCore;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly IBackgroundTaskQueue _taskQueue;

        public NotificationService(AppDbContext context, EmailService emailService, IBackgroundTaskQueue taskQueue)
        {
            _context = context;
            _emailService = emailService;
            _taskQueue = taskQueue;
        }

        public async Task AddNotificationAsync(NotificationDTO notificationDto, string userId)
        {
            var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Email })
            .FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("User email not found.");
            }

            var notification = new Notifications
            {
                UserId = userId,
                PostId = notificationDto.PostId,
                NotificationText = notificationDto.Content,
                IsRead = false,
                NotificationDate = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _taskQueue.QueueEmail(async () =>
            {
                await _emailService.SendEmailAsync(user.Email, "New Notification", notificationDto.Content);
            });
        }



        public async Task<NotificationResponseDTO> GetNotificationsAsync(string userId)
        {
            // Check if there is at least one unread notification
            bool hasUnread = await _context.Notifications
                .AnyAsync(n => n.UserId == userId && !n.IsRead);

            // Fetch all notifications for the user
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.NotificationDate)
                .Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    Content = n.NotificationText,
                    NotificationDate = n.NotificationDate,
                    IsRead = n.IsRead // Return the actual `IsRead` value for each notification
                })
                .ToListAsync();

            return new NotificationResponseDTO
            {
                HasUnread = hasUnread, // True if at least one notification is unread
                Notifications = notifications
            };
        }




        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
