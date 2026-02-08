using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryImageImportQueue : IStoryImageImportQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryImageImportQueue> _logger;

    public StoryImageImportQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryImageImportQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["ImageImport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-image-import-queue");
    }

    public async Task EnqueueAsync(StoryImageImportJob job, CancellationToken ct = default)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new { JobId = job.Id, StoryId = job.StoryId };
            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryImageImportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryImageImportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryImageImportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }
}
