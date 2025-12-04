using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class StoryPublishQueue : IStoryPublishQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<StoryPublishQueue> _logger;

    public StoryPublishQueue(IConfiguration configuration, ILogger<StoryPublishQueue> logger)
    {
        _logger = logger;

        var section = configuration.GetSection("AzureStorage");
        var connectionString = ResolveConfiguredValue(section["ConnectionString"])
                               ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
        }

        var queueName = ResolveConfiguredValue(section["PublishQueueName"])
                        ?? Environment.GetEnvironmentVariable("AzureStorage__PublishQueueName")
                        ?? "story-publish-queue";

        _queueClient = new QueueClient(connectionString, queueName, new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64
        });
    }

    public async Task EnqueueAsync(StoryPublishJob job, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Creating queue if not exists: queueName={QueueName}", _queueClient.Name);
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,  // Must match StoryPublishQueuePayload property name
                StoryId = job.StoryId,
                DraftVersion = job.DraftVersion
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing StoryPublishJob: jobId={JobId} storyId={StoryId} draftVersion={DraftVersion} queueName={QueueName}",
                job.Id, job.StoryId, job.DraftVersion, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
            
            _logger.LogInformation("Successfully enqueued StoryPublishJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue StoryPublishJob: jobId={JobId} storyId={StoryId} queueName={QueueName}",
                job.Id, job.StoryId, _queueClient.Name);
            throw;
        }
    }

    private static string? ResolveConfiguredValue(string? configured)
    {
        if (string.IsNullOrWhiteSpace(configured))
        {
            return null;
        }

        if (configured.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
        {
            var envKey = configured[4..].Trim();
            return string.IsNullOrWhiteSpace(envKey) ? null : Environment.GetEnvironmentVariable(envKey);
        }

        return configured;
    }
}


