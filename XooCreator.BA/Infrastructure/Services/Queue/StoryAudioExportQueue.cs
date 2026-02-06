using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryAudioExportQueue : IStoryAudioExportQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryAudioExportQueue> _logger;

    public StoryAudioExportQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryAudioExportQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AudioExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-audio-export-queue");
    }

    public async Task EnqueueAsync(StoryAudioExportJob job, CancellationToken ct = default)
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

            _logger.LogInformation("Enqueuing StoryAudioExportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryAudioExportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryAudioExportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }
}
