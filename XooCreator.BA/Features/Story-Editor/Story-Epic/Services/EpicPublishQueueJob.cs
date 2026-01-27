using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicPublishQueueJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EpicPublishQueueJob> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public EpicPublishQueueJob(
        IServiceProvider services,
        ILogger<EpicPublishQueueJob> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicPublish"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-publish-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Log immediately on startup to confirm worker is starting
        _logger.LogInformation("EpicPublishQueueJob initializing... QueueName={QueueName}", _queueClient.Name);
        
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("EpicPublishQueueJob started successfully. QueueName={QueueName} QueueUri={QueueUri}", 
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EpicPublishQueueJob failed to create/connect to queue. QueueName={QueueName}. Worker will retry in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromSeconds(60), cancellationToken: stoppingToken);
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

                EpicPublishQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<EpicPublishQueuePayload>(message.MessageText);
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
                    var publisher = scope.ServiceProvider.GetRequiredService<IStoryEpicPublishingService>();
                    var marketplaceCacheInvalidator = scope.ServiceProvider.GetService<IMarketplaceCacheInvalidator>();

                    var job = await db.EpicPublishJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is EpicPublishJobStatus.Completed or EpicPublishJobStatus.Failed or EpicPublishJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.EpicPublish, job.Id, new
                        {
                            jobId = job.Id,
                            epicId = job.EpicId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            dequeueCount = job.DequeueCount,
                            draftVersion = job.DraftVersion
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = EpicPublishJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        // Load craft with all necessary includes for publishing
                        var craft = await db.StoryEpicCrafts
                            .Include(c => c.Regions)
                            .Include(c => c.StoryNodes)
                            .Include(c => c.UnlockRules)
                            .Include(c => c.Translations)
                            .AsSplitQuery() // Use split query to reduce memory pressure
                            .FirstOrDefaultAsync(c => c.Id == job.EpicId, stoppingToken);

                        if (craft == null)
                        {
                            job.Status = EpicPublishJobStatus.Failed;
                            job.ErrorMessage = "StoryEpicCraft not found.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                        }
                        else if (craft.LastDraftVersion < job.DraftVersion)
                        {
                            job.Status = EpicPublishJobStatus.Failed;
                            job.ErrorMessage = $"Draft version {job.DraftVersion} no longer available (LastDraftVersion={craft.LastDraftVersion}).";
                            job.CompletedAtUtc = DateTime.UtcNow;
                        }
                        else if (craft.LastDraftVersion > job.DraftVersion)
                        {
                            job.Status = EpicPublishJobStatus.Superseded;
                            job.ErrorMessage = $"Job draftVersion={job.DraftVersion} superseded by newer draftVersion={craft.LastDraftVersion}.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                        }
                        else
                        {
                            _logger.LogInformation("Processing EpicPublishJob: jobId={JobId} epicId={EpicId} draftVersion={DraftVersion} forceFull={ForceFull}",
                                job.Id, job.EpicId, job.DraftVersion, job.ForceFull);

                            var langTag = job.LangTag?.ToLowerInvariant() ?? "ro-ro";
                            
                            try
                            {
                                await publisher.PublishFromCraftAsync(craft, job.RequestedByEmail, langTag, job.ForceFull, job.IsAdminPublish, stoppingToken);
                                _logger.LogInformation("PublishFromCraftAsync completed successfully: jobId={JobId} epicId={EpicId}", job.Id, job.EpicId);
                            }
                            catch (Exception publishEx)
                            {
                                _logger.LogError(publishEx, "PublishFromCraftAsync failed: jobId={JobId} epicId={EpicId}", job.Id, job.EpicId);
                                throw; // Re-throw to trigger retry logic
                            }

                            job.Status = EpicPublishJobStatus.Completed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            job.ErrorMessage = null;
                            
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            _logger.LogInformation("Job status saved as Completed: jobId={JobId}", job.Id);
                            
                            // Reset marketplace cache after successful epic publish (safe: all locales).
                            // Best-effort: do not affect job completion if cache reset fails.
                            try
                            {
                                marketplaceCacheInvalidator?.ResetAll();
                            }
                            catch (Exception cacheEx)
                            {
                                _logger.LogWarning(cacheEx, "Failed to reset marketplace cache after epic publish. epicId={EpicId} jobId={JobId}", job.EpicId, job.Id);
                            }

                            // Send newsletter for new epics (best-effort, don't affect job completion)
                            try
                            {
                                var newsletterService = scope.ServiceProvider.GetRequiredService<XooCreator.BA.Features.User.Services.INewsletterAutoSendService>();
                                await newsletterService.TrySendNewsletterForNewEpicAsync(craft.Id, langTag, stoppingToken);
                            }
                            catch (Exception newsletterEx)
                            {
                                _logger.LogWarning(newsletterEx, "Failed to send newsletter for new epic: epicId={EpicId}", craft.Id);
                            }

                            // Cleanup: Delete StoryEpicCraft and draft assets after successful publish
                            // This is done in a separate try-catch to not affect the job status
                            try
                            {
                                _logger.LogDebug("Starting cleanup for epicId={EpicId}", craft.Id);
                                
                                // Reload craft in current context to ensure it's tracked
                                var craftToDelete = await db.StoryEpicCrafts
                                    .Include(c => c.Regions)
                                    .Include(c => c.StoryNodes)
                                    .Include(c => c.UnlockRules)
                                    .Include(c => c.Translations)
                                    .FirstOrDefaultAsync(c => c.Id == craft.Id, stoppingToken);
                                
                                if (craftToDelete != null)
                                {
                                    // Remove related entities first to avoid FK violations
                                    db.RemoveRange(craftToDelete.Regions);
                                    db.RemoveRange(craftToDelete.StoryNodes);
                                    db.RemoveRange(craftToDelete.UnlockRules);
                                    db.RemoveRange(craftToDelete.Translations);
                                    await db.SaveChangesAsync(stoppingToken);
                                    
                                    db.StoryEpicCrafts.Remove(craftToDelete);
                                    await db.SaveChangesAsync(stoppingToken);
                                    _logger.LogInformation("Cleaned up StoryEpicCraft after publish: epicId={EpicId}", craft.Id);
                                }
                                else
                                {
                                    _logger.LogWarning("StoryEpicCraft not found for cleanup: epicId={EpicId}", craft.Id);
                                }
                            }
                            catch (Exception cleanupEx)
                            {
                                _logger.LogWarning(cleanupEx, "Failed to cleanup draft for epicId={EpicId}, but publish succeeded. Manual cleanup may be needed.", craft.Id);
                                // Don't fail the job if cleanup fails - publish was successful
                            }

                            _logger.LogInformation("EpicPublishJob fully completed: jobId={JobId} epicId={EpicId} draftVersion={DraftVersion}",
                                job.Id, job.EpicId, job.DraftVersion);
                        }

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process EpicPublishJob: jobId={JobId} epicId={EpicId}", job.Id, job.EpicId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = EpicPublishJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        // If DequeueCount < 3, don't delete message - queue will re-deliver it
                    }
                } // Scope disposed here - DbContext and all scoped services are cleaned up
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicPublishQueueJob stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicPublishQueueJob stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in EpicPublishQueueJob. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in EpicPublishQueueJob loop. ExceptionType={ExceptionType}. Worker will continue after 10 seconds delay.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        
        _logger.LogInformation("EpicPublishQueueJob has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record EpicPublishQueuePayload(Guid JobId, string EpicId, int DraftVersion);
}

