using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryForkAssetsQueue : IStoryForkAssetsQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryForkAssetsQueue> _logger;

    public StoryForkAssetsQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryForkAssetsQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["ForkAssets"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-fork-assets-queue");
    }

    public async Task EnqueueAsync(StoryForkAssetJob job, CancellationToken ct = default)
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

            _logger.LogInformation("Enqueuing StoryForkAssetJob: jobId={JobId} targetStoryId={TargetStoryId} queueName={QueueName}",
                job.Id, job.TargetStoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryForkAssetJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryForkAssetJob: jobId={JobId} targetStoryId={TargetStoryId} queueName={QueueName}",
                job.Id, job.TargetStoryId, _queueClient.Name);
            throw;
        }
    }
}

