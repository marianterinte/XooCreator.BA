using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryPublishQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryPublishQueueWorker> _logger;
    private readonly QueueClient _queueClient;

    public StoryPublishQueueWorker(
        IServiceProvider services,
        ILogger<StoryPublishQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Publish"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-publish-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        _logger.LogInformation("StoryPublishQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}", 
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

                    var job = await db.StoryPublishJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryPublishJobStatus.Completed or StoryPublishJobStatus.Failed or StoryPublishJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryPublishJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);

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

                            var langTag = job.LangTag.ToLowerInvariant();
                            await publisher.UpsertFromCraftAsync(craft, job.RequestedByEmail, langTag, job.ForceFull, stoppingToken);

                            job.Status = StoryPublishJobStatus.Completed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            job.ErrorMessage = null;
                            
                            await db.SaveChangesAsync(stoppingToken);

                            // Cleanup: Delete StoryCraft and draft assets after successful publish
                            try
                            {
                                _logger.LogInformation("Cleaning up draft after successful publish: storyId={StoryId}", job.StoryId);
                                await cleanupService.DeleteDraftAssetsAsync(job.RequestedByEmail, job.StoryId, stoppingToken);
                                await crafts.DeleteAsync(job.StoryId, stoppingToken);
                                _logger.LogInformation("Draft cleanup completed: storyId={StoryId}", job.StoryId);
                            }
                            catch (Exception cleanupEx)
                            {
                                // Log cleanup errors but don't fail the job - publish was successful
                                _logger.LogWarning(cleanupEx, "Failed to cleanup draft after publish: storyId={StoryId}", job.StoryId);
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
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
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                    }
                } // Scope disposed here - DbContext and all scoped services are cleaned up
            }
            catch (TaskCanceledException)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryPublishQueueWorker loop.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record StoryPublishQueuePayload(Guid JobId, string StoryId, int DraftVersion);
}


