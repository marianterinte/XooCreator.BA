using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IAnimalVersionQueue
{
    Task EnqueueAsync(AnimalVersionJob job, CancellationToken ct = default);
}

public class AnimalVersionQueue : IAnimalVersionQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<AnimalVersionQueue> _logger;

    public AnimalVersionQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<AnimalVersionQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AnimalVersion"] ?? "animal-version-jobs";
        _queueClient = queueClientFactory.CreateClient(queueName, "animal-version-queue");
    }

    public async Task EnqueueAsync(AnimalVersionJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                AnimalId = job.AnimalId,
                BaseVersion = job.BaseVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing AnimalVersionJob: jobId={JobId} animalId={AnimalId} baseVersion={BaseVersion} queueName={QueueName}",
                job.Id, job.AnimalId, job.BaseVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued AnimalVersionJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue AnimalVersionJob: jobId={JobId} animalId={AnimalId}", job.Id, job.AnimalId);
            throw;
        }
    }
}
