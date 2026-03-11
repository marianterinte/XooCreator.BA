using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class GenerativeLoiQueue : IGenerativeLoiQueue
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<GenerativeLoiQueue> _logger;

    public GenerativeLoiQueue(
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<GenerativeLoiQueue> logger)
    {
        _logger = logger;
        var queueName = configuration.GetSection("AzureStorage:Queues")?["GenerativeLoi"];
        _queueClient = queueClientFactory.CreateClient(queueName, "generative-loi-queue");
    }

    public async Task EnqueueAsync(GenerativeLoiJob job, CancellationToken ct = default)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);
            var payload = new { JobId = job.Id };
            var messageText = JsonSerializer.Serialize(payload);
            _logger.LogInformation("Enqueuing GenerativeLoiJob: jobId={JobId} userId={UserId}", job.Id, job.UserId);
            await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue GenerativeLoiJob: jobId={JobId}", job.Id);
            throw;
        }
    }
}
