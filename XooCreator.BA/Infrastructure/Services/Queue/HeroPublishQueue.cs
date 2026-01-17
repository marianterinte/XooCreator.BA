using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class HeroPublishQueue : IHeroPublishQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<HeroPublishQueue> _logger;

    public HeroPublishQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<HeroPublishQueue> logger)
    {
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["HeroPublish"] ?? "hero-publish-queue";
        _queueClient = queueClientFactory.CreateClient(queueName, "hero-publish-queue");
    }

    public async Task EnqueueAsync(HeroPublishJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                HeroId = job.HeroId,
                // Hero definition doesn't have a DraftVersion concept in the same way as Story, 
                // but we might add it or just ignore it in the worker if not needed.
                // However, let's keep it consistent if the job has it.
                // But HeroPublishJob entity doesn't have DraftVersion in my previous file creation?
                // Let me check. I didn't add DraftVersion to HeroPublishJob.
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing HeroPublishJob: jobId={JobId} heroId={HeroId} queueName={QueueName}",
                job.Id, job.HeroId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued HeroPublishJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue HeroPublishJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);
            throw;
        }
    }
}
