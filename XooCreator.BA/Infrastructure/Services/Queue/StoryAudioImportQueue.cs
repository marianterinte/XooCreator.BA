using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryAudioImportQueue : IStoryAudioImportQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryAudioImportQueue> _logger;

    public StoryAudioImportQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryAudioImportQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AudioImport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-audio-import-queue");
    }

    public async Task EnqueueAsync(StoryAudioImportJob job, CancellationToken ct = default)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new { JobId = job.Id, StoryId = job.StoryId };
            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryAudioImportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryAudioImportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryAudioImportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }
}
