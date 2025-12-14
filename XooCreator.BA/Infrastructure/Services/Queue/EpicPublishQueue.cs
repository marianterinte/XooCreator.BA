using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class EpicPublishQueue : IEpicPublishQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<EpicPublishQueue> _logger;

    public EpicPublishQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<EpicPublishQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicPublish"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-publish-queue");
    }

    public async Task EnqueueAsync(EpicPublishJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,  // Must match EpicPublishQueuePayload property name
                EpicId = job.EpicId,
                DraftVersion = job.DraftVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing EpicPublishJob: jobId={JobId} epicId={EpicId} draftVersion={DraftVersion} queueName={QueueName}",
                job.Id, job.EpicId, job.DraftVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued EpicPublishJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue EpicPublishJob: jobId={JobId} epicId={EpicId}", job.Id, job.EpicId);
            throw;
        }
    }
}

