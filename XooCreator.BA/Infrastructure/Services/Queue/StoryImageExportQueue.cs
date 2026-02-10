using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryImageExportQueue : IStoryImageExportQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryImageExportQueue> _logger;

    public StoryImageExportQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryImageExportQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["ImageExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-image-export-queue");
    }

    public async Task EnqueueAsync(StoryImageExportJob job, CancellationToken ct = default)
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

            _logger.LogInformation("Enqueuing StoryImageExportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryImageExportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryImageExportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }
}
