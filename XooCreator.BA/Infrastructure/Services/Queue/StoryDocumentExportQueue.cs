using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryDocumentExportQueue : IStoryDocumentExportQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryDocumentExportQueue> _logger;

    public StoryDocumentExportQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<StoryDocumentExportQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["DocumentExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-document-export-queue");
    }

    public async Task EnqueueAsync(StoryDocumentExportJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new StoryDocumentExportQueuePayload(job.Id, job.StoryId);
            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryDocumentExportJob: jobId={JobId} storyId={StoryId} format={Format} isDraft={IsDraft} queueName={QueueName}",
                job.Id, job.StoryId, job.Format, job.IsDraft, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued StoryDocumentExportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryDocumentExportJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }

    private sealed record StoryDocumentExportQueuePayload(Guid JobId, string StoryId);
}

