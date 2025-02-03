namespace CommunityKnowledgeSharingPlatform.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void QueueEmail(Func<Task> work);
        Task ProcessQueueAsync(CancellationToken cancellationToken);
    }


}
