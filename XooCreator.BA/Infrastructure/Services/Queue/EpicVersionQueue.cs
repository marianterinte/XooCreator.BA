using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class EpicVersionQueue : IEpicVersionQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<EpicVersionQueue> _logger;

    public EpicVersionQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<EpicVersionQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicVersion"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-version-queue");
    }

    public async Task EnqueueAsync(EpicVersionJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,  // Must match EpicVersionQueuePayload property name
                EpicId = job.EpicId,
                BaseVersion = job.BaseVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing EpicVersionJob: jobId={JobId} epicId={EpicId} baseVersion={BaseVersion} queueName={QueueName}",
                job.Id, job.EpicId, job.BaseVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued EpicVersionJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue EpicVersionJob: jobId={JobId} epicId={EpicId}", job.Id, job.EpicId);
            throw;
        }
    }
}

