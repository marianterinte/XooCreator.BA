using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryExportQueue : IStoryExportQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryExportQueue> _logger;

    public StoryExportQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryExportQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Export"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-export-full-export-queue");
    }

    public async Task EnqueueAsync(StoryExportJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                StoryId = job.StoryId,
                IsDraft = job.IsDraft
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryExportJob: jobId={JobId} storyId={StoryId} isDraft={IsDraft} queueName={QueueName}",
                job.Id, job.StoryId, job.IsDraft, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued StoryExportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryExportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }
}
