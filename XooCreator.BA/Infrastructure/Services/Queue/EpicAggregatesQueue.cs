using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class EpicAggregatesQueue : IEpicAggregatesQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<EpicAggregatesQueue> _logger;

    public EpicAggregatesQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<EpicAggregatesQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicAggregates"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-aggregates-queue");
    }

    public async Task EnqueueHeroVersionAsync(HeroVersionJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobType = "HeroVersion",
                JobId = job.Id,
                HeroId = job.HeroId,
                BaseVersion = job.BaseVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing HeroVersionJob: jobId={JobId} heroId={HeroId} baseVersion={BaseVersion} queueName={QueueName}",
                job.Id, job.HeroId, job.BaseVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued HeroVersionJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue HeroVersionJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);
            throw;
        }
    }

    public async Task EnqueueRegionVersionAsync(RegionVersionJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobType = "RegionVersion",
                JobId = job.Id,
                RegionId = job.RegionId,
                BaseVersion = job.BaseVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing RegionVersionJob: jobId={JobId} regionId={RegionId} baseVersion={BaseVersion} queueName={QueueName}",
                job.Id, job.RegionId, job.BaseVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued RegionVersionJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue RegionVersionJob: jobId={JobId} regionId={RegionId}", job.Id, job.RegionId);
            throw;
        }
    }
}

