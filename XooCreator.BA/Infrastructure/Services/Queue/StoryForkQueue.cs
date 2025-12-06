using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryForkQueue : IStoryForkQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryForkQueue> _logger;

    public StoryForkQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryForkQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Fork"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-fork-queue");
    }

    public async Task EnqueueAsync(StoryForkJob job, CancellationToken ct = default)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                TargetStoryId = job.TargetStoryId
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryForkJob: jobId={JobId} targetStoryId={StoryId} queueName={QueueName}",
                job.Id, job.TargetStoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryForkJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryForkJob: jobId={JobId} targetStoryId={StoryId} queueName={QueueName}",
                job.Id, job.TargetStoryId, _queueClient.Name);
            throw;
        }
    }
}

