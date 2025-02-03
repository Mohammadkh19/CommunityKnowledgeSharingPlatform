using CommunityKnowledgeSharingPlatform.Interfaces;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public EmailBackgroundService(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _taskQueue.ProcessQueueAsync(stoppingToken);
        }
    }

}
