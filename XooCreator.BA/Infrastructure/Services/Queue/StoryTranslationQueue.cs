using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryTranslationQueue : IStoryTranslationQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryTranslationQueue> _logger;

    public StoryTranslationQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryTranslationQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Translation"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-translation-queue");
    }

    public async Task EnqueueAsync(StoryTranslationJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                StoryId = job.StoryId
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryTranslationJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryTranslationJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryTranslationJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }
}
