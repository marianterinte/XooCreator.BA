using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class AnimalPublishQueue : IAnimalPublishQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<AnimalPublishQueue> _logger;

    public AnimalPublishQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<AnimalPublishQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AnimalPublish"] ?? "animal-publish-queue";
        _queueClient = queueClientFactory.CreateClient(queueName, "animal-publish-queue");
    }

    public async Task EnqueueAsync(AnimalPublishJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                AnimalId = job.AnimalId
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing AnimalPublishJob: jobId={JobId} animalId={AnimalId} queueName={QueueName}",
                job.Id, job.AnimalId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued AnimalPublishJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue AnimalPublishJob: jobId={JobId} animalId={AnimalId}", job.Id, job.AnimalId);
            throw;
        }
    }
}
