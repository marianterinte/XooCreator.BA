using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IHeroDefinitionVersionQueue
{
    Task EnqueueAsync(HeroDefinitionVersionJob job, CancellationToken ct = default);
}

public class HeroDefinitionVersionQueue : IHeroDefinitionVersionQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<HeroDefinitionVersionQueue> _logger;

    public HeroDefinitionVersionQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<HeroDefinitionVersionQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["HeroVersion"] ?? "hero-version-jobs";
        _queueClient = queueClientFactory.CreateClient(queueName, "hero-version-queue");
    }

    public async Task EnqueueAsync(HeroDefinitionVersionJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                HeroId = job.HeroId,
                BaseVersion = job.BaseVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing HeroDefinitionVersionJob: jobId={JobId} heroId={HeroId} baseVersion={BaseVersion} queueName={QueueName}",
                job.Id, job.HeroId, job.BaseVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued HeroDefinitionVersionJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue HeroDefinitionVersionJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);
            throw;
        }
    }
}
