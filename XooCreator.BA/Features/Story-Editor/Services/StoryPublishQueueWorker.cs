using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryPublishQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryPublishQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public StoryPublishQueueWorker(
        IServiceProvider services,
        ILogger<StoryPublishQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Publish"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-publish-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryPublishQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryPublishQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}", 
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryPublishQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
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

                StoryPublishQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryPublishQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.StoryId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                // Create scope for each message to ensure proper disposal of scoped services (DbContext, etc.)
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IStoryPublishingService>();
                    var crafts = scope.ServiceProvider.GetRequiredService<IStoryCraftsRepository>();
                    var cleanupService = scope.ServiceProvider.GetRequiredService<IStoryDraftAssetCleanupService>();
                    var marketplaceCacheInvalidator = scope.ServiceProvider.GetRequiredService<IMarketplaceCacheInvalidator>();
                    var storiesRepository = scope.ServiceProvider.GetRequiredService<IStoriesRepository>();

                    var job = await db.StoryPublishJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryPublishJobStatus.Completed or StoryPublishJobStatus.Failed or StoryPublishJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.StoryPublish, job.Id, new
                        {
                            jobId = job.Id,
                            storyId = job.StoryId,
                            status = job.Status.ToString(),
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            dequeueCount = job.DequeueCount
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryPublishJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        // Load craft with all necessary includes for publishing
                        // Note: This loads significant data into memory, but it's necessary for publish operation
                        var craft = await db.StoryCrafts
                            .Include(c => c.Tiles).ThenInclude(t => t.Translations)
                            .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
                            .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                            .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
                            .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
                            .AsSplitQuery() // Use split query to reduce memory pressure
                            .FirstOrDefaultAsync(c => c.StoryId == job.StoryId, stoppingToken);

                        if (craft == null)
                        {
                            job.Status = StoryPublishJobStatus.Failed;
                            job.ErrorMessage = "StoryCraft not found.";
                        }
                        else if (craft.LastDraftVersion < job.DraftVersion)
                        {
                            job.Status = StoryPublishJobStatus.Failed;
                            job.ErrorMessage = $"Draft version {job.DraftVersion} no longer available (LastDraftVersion={craft.LastDraftVersion}).";
                        }
                        else if (craft.LastDraftVersion > job.DraftVersion)
                        {
                            job.Status = StoryPublishJobStatus.Superseded;
                            job.ErrorMessage = $"Job draftVersion={job.DraftVersion} superseded by newer draftVersion={craft.LastDraftVersion}.";
                        }
                        else
                        {
                            _logger.LogInformation("Processing StoryPublishJob: jobId={JobId} storyId={StoryId} draftVersion={DraftVersion} forceFull={ForceFull}",
                                job.Id, job.StoryId, job.DraftVersion, job.ForceFull);

                            var langTag = job.LangTag?.ToLowerInvariant() ?? "ro-ro";

                            // Get owner email from craft.OwnerUserId (for admin publishing another user's draft)
                            // This ensures assets are copied from/to the correct owner's folder
                            string ownerEmail = job.RequestedByEmail;
                            if (craft.OwnerUserId != Guid.Empty)
                            {
                                var ownerUser = await db.Set<AlchimaliaUser>()
                                    .AsNoTracking()
                                    .Where(u => u.Id == craft.OwnerUserId)
                                    .Select(u => u.Email)
                                    .FirstOrDefaultAsync(stoppingToken);
                                
                                if (!string.IsNullOrWhiteSpace(ownerUser))
                                {
                                    ownerEmail = ownerUser;
                                    _logger.LogInformation("Using owner email for publish: storyId={StoryId} ownerEmail={OwnerEmail} requestedBy={RequestedBy}", 
                                        job.StoryId, ownerEmail, job.RequestedByEmail);
                                }
                            }

                            try
                            {
                                await publisher.UpsertFromCraftAsync(craft, ownerEmail, langTag, job.ForceFull, stoppingToken);
                                _logger.LogInformation("UpsertFromCraftAsync completed successfully: jobId={JobId} storyId={StoryId} ownerEmail={OwnerEmail}", 
                                    job.Id, job.StoryId, ownerEmail);
                            }
                            catch (Exception publishEx)
                            {
                                _logger.LogError(publishEx, "UpsertFromCraftAsync failed: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
                                throw;
                            }

                            job.Status = StoryPublishJobStatus.Completed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            job.ErrorMessage = null;
                            
                            await db.SaveChangesAsync(stoppingToken);

                            // Invalidate caches so readers see the new version: story content cache + marketplace catalog.
                            try
                            {
                                storiesRepository.InvalidateStoryContentCache(job.StoryId);
                                marketplaceCacheInvalidator.ResetAll();
                                _logger.LogInformation("Cache invalidated after story publish: storyId={StoryId} jobId={JobId}", job.StoryId, job.Id);
                            }
                            catch (Exception cacheEx)
                            {
                                _logger.LogWarning(cacheEx, "Failed to invalidate cache after story publish. storyId={StoryId} jobId={JobId}", job.StoryId, job.Id);
                            }

                            try
                            {
                                _logger.LogInformation("Cleaning up draft after successful publish: storyId={StoryId} ownerEmail={OwnerEmail}", 
                                    job.StoryId, ownerEmail);
                                await cleanupService.DeleteDraftAssetsAsync(ownerEmail, job.StoryId, stoppingToken);
                                await crafts.DeleteAsync(job.StoryId, stoppingToken);
                                _logger.LogInformation("Draft cleanup completed: storyId={StoryId}", job.StoryId);
                            }
                            catch (Exception cleanupEx)
                            {
                                _logger.LogWarning(cleanupEx, "Failed to cleanup draft after publish: storyId={StoryId}", job.StoryId);
                            }

                            // Send newsletter for new stories (best-effort, don't affect job completion)
                            try
                            {
                                var newsletterService = scope.ServiceProvider.GetRequiredService<XooCreator.BA.Features.User.Services.INewsletterAutoSendService>();
                                await newsletterService.TrySendNewsletterForNewStoryAsync(job.StoryId, langTag, stoppingToken);
                            }
                            catch (Exception newsletterEx)
                            {
                                _logger.LogWarning(newsletterEx, "Failed to send newsletter for new story: storyId={StoryId}", job.StoryId);
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process StoryPublishJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryPublishJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                    }
                } // Scope disposed here - DbContext and all scoped services are cleaned up
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryPublishQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryPublishQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in StoryPublishQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryPublishQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryPublishQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record StoryPublishQueuePayload(Guid JobId, string StoryId, int DraftVersion);
}


