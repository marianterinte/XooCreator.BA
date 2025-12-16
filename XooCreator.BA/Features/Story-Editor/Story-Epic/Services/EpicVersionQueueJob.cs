using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicVersionQueueJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EpicVersionQueueJob> _logger;
    private readonly QueueClient _queueClient;

    public EpicVersionQueueJob(
        IServiceProvider services,
        ILogger<EpicVersionQueueJob> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicVersion"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-version-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        _logger.LogInformation("EpicVersionQueueJob started. QueueName={QueueName} QueueUri={QueueUri}", 
            _queueClient.Name, _queueClient.Uri);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromSeconds(30), cancellationToken: stoppingToken);
                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    // Log only every 10th check to avoid spam (every ~30 seconds)
                    if (DateTime.UtcNow.Second % 30 < 3)
                    {
                        _logger.LogDebug("No messages in queue. QueueName={QueueName}", _queueClient.Name);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                if (message?.MessageId == null)
                {
                    _logger.LogWarning("Received message with null MessageId. QueueName={QueueName}", _queueClient.Name);
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Received message from queue: messageId={MessageId} queueName={QueueName}", 
                    message.MessageId, _queueClient.Name);

                EpicVersionQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<EpicVersionQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.EpicId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                // Create scope for each message to ensure proper disposal of scoped services (DbContext, etc.)
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var epicService = scope.ServiceProvider.GetRequiredService<IStoryEpicService>();

                    var job = await db.EpicVersionJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is EpicVersionJobStatus.Completed or EpicVersionJobStatus.Failed or EpicVersionJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    // Check if there's another running job for the same epic
                    var runningJob = await db.EpicVersionJobs
                        .FirstOrDefaultAsync(j => j.EpicId == job.EpicId && j.Id != job.Id && j.Status == EpicVersionJobStatus.Running, stoppingToken);
                    if (runningJob != null)
                    {
                        job.Status = EpicVersionJobStatus.Superseded;
                        job.ErrorMessage = $"Job superseded by running job {runningJob.Id}.";
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = EpicVersionJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);

                    try
                    {
                        // Load published StoryEpicDefinition with all necessary includes
                        var definition = await db.StoryEpicDefinitions
                            .Include(d => d.Regions)
                            .Include(d => d.StoryNodes)
                            .Include(d => d.UnlockRules)
                            .Include(d => d.Translations)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(d => d.Id == job.EpicId, stoppingToken);

                        if (definition == null)
                        {
                            job.Status = EpicVersionJobStatus.Failed;
                            job.ErrorMessage = "StoryEpicDefinition not found.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        if (definition.Status != "published")
                        {
                            job.Status = EpicVersionJobStatus.Failed;
                            job.ErrorMessage = $"Epic is not published (status: {definition.Status}).";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        if (definition.Version != job.BaseVersion)
                        {
                            job.Status = EpicVersionJobStatus.Superseded;
                            job.ErrorMessage = $"Epic version changed from {job.BaseVersion} to {definition.Version}.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        // Check if draft already exists
                        var existingCraft = await db.StoryEpicCrafts.FirstOrDefaultAsync(c => c.Id == job.EpicId, stoppingToken);
                        if (existingCraft != null && existingCraft.Status != "published")
                        {
                            job.Status = EpicVersionJobStatus.Failed;
                            job.ErrorMessage = "A draft already exists for this epic. Please edit or publish it first.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        _logger.LogInformation("Processing EpicVersionJob: jobId={JobId} epicId={EpicId} baseVersion={BaseVersion}",
                            job.Id, job.EpicId, job.BaseVersion);

                        // Create StoryEpicCraft from StoryEpicDefinition
                        // TODO: Implement CreateVersionFromPublishedAsync in IStoryEpicService
                        await epicService.CreateVersionFromPublishedAsync(job.OwnerUserId, job.EpicId, stoppingToken);

                        job.Status = EpicVersionJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("EpicVersionJob completed: jobId={JobId} epicId={EpicId} baseVersion={BaseVersion}",
                            job.Id, job.EpicId, job.BaseVersion);

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process EpicVersionJob: jobId={JobId} epicId={EpicId}", job.Id, job.EpicId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = EpicVersionJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        // If DequeueCount < 3, don't delete message - queue will re-deliver it
                    }
                } // Scope disposed here - DbContext and all scoped services are cleaned up
            }
            catch (TaskCanceledException)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in EpicVersionQueueJob loop.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record EpicVersionQueuePayload(Guid JobId, string EpicId, int BaseVersion);
}

