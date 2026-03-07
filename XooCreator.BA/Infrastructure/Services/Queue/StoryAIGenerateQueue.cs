using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryAIGenerateQueue : IStoryAIGenerateQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryAIGenerateQueue> _logger;

    public StoryAIGenerateQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryAIGenerateQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AIGenerate"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-ai-generate-queue");
    }

    public async Task EnqueueAsync(StoryAIGenerateJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new { JobId = job.Id };
            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryAIGenerateJob: jobId={JobId} queueName={QueueName}",
                job.Id, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryAIGenerateJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryAIGenerateJob: jobId={JobId} queueName={QueueName}",
                job.Id, _queueClient.Name);
            throw;
        }
    }
}
