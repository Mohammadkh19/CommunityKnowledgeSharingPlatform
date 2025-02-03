using System.Collections.Concurrent;
using CommunityKnowledgeSharingPlatform.Interfaces;

namespace CommunityKnowledgeSharingPlatform.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<Task>> _emailTasks = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void QueueEmail(Func<Task> work)
        {
            _emailTasks.Enqueue(work);
            _signal.Release(); // Notify worker
        }

        public async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _signal.WaitAsync(cancellationToken);

                if (_emailTasks.TryDequeue(out var work))
                {
                    try
                    {
                        await work();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email sending failed: {ex.Message}");
                    }
                }
            }
        }
    }
}
