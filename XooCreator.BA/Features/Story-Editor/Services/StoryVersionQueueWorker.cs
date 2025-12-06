using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryVersionQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryVersionQueueWorker> _logger;
    private readonly QueueClient _queueClient;

    public StoryVersionQueueWorker(
        IServiceProvider services,
        ILogger<StoryVersionQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Version"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-version-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        _logger.LogInformation("StoryVersionQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}", 
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

                StoryVersionQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryVersionQueuePayload>(message.MessageText);
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
                    var storyCopyService = scope.ServiceProvider.GetRequiredService<IStoryCopyService>();
                    var storyAssetCopyService = scope.ServiceProvider.GetRequiredService<IStoryAssetCopyService>();
                    var crafts = scope.ServiceProvider.GetRequiredService<IStoryCraftsRepository>();

                    var job = await db.StoryVersionJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryVersionJobStatus.Completed or StoryVersionJobStatus.Failed or StoryVersionJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    // Check if there's another running job for the same story
                    var runningJob = await db.StoryVersionJobs
                        .FirstOrDefaultAsync(j => j.StoryId == job.StoryId && j.Id != job.Id && j.Status == StoryVersionJobStatus.Running, stoppingToken);
                    if (runningJob != null)
                    {
                        job.Status = StoryVersionJobStatus.Superseded;
                        job.ErrorMessage = $"Job superseded by running job {runningJob.Id}.";
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryVersionJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);

                    try
                    {
                        // Load published StoryDefinition with all necessary includes
                        var definition = await db.StoryDefinitions
                            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
                            .Include(d => d.Translations)
                            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
                            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(d => d.StoryId == job.StoryId, stoppingToken);

                        if (definition == null)
                        {
                            job.Status = StoryVersionJobStatus.Failed;
                            job.ErrorMessage = "StoryDefinition not found.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        if (definition.Status != StoryStatus.Published)
                        {
                            job.Status = StoryVersionJobStatus.Failed;
                            job.ErrorMessage = $"Story is not published (status: {definition.Status}).";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        if (definition.Version != job.BaseVersion)
                        {
                            job.Status = StoryVersionJobStatus.Superseded;
                            job.ErrorMessage = $"Story version changed from {job.BaseVersion} to {definition.Version}.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        // Check if draft already exists
                        var existingCraft = await crafts.GetAsync(job.StoryId, stoppingToken);
                        if (existingCraft != null && existingCraft.Status != StoryStatus.Published.ToDb())
                        {
                            job.Status = StoryVersionJobStatus.Failed;
                            job.ErrorMessage = "A draft already exists for this story. Please edit or publish it first.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        _logger.LogInformation("Processing StoryVersionJob: jobId={JobId} storyId={StoryId} baseVersion={BaseVersion}",
                            job.Id, job.StoryId, job.BaseVersion);

                        // Create StoryCraft from StoryDefinition
                        var newCraft = await storyCopyService.CreateCopyFromDefinitionAsync(definition, job.OwnerUserId, job.StoryId, stoppingToken);

                        // Copy assets from published to draft storage
                        try
                        {
                            var assets = storyAssetCopyService.CollectFromDefinition(definition);
                            await storyAssetCopyService.CopyPublishedToDraftAsync(
                                assets,
                                job.RequestedByEmail,
                                definition.StoryId,
                                job.RequestedByEmail,
                                job.StoryId,
                                stoppingToken);
                        }
                        catch (Exception assetEx)
                        {
                            _logger.LogWarning(assetEx, "Failed to copy assets during create version for storyId={StoryId}, but continuing", job.StoryId);
                            // Don't fail the job if asset copy fails - craft was created successfully
                        }

                        job.Status = StoryVersionJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("StoryVersionJob completed: jobId={JobId} storyId={StoryId} baseVersion={BaseVersion}",
                            job.Id, job.StoryId, job.BaseVersion);

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process StoryVersionJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryVersionJobStatus.Failed;
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
                _logger.LogError(ex, "Unexpected error in StoryVersionQueueWorker loop.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record StoryVersionQueuePayload(Guid JobId, string StoryId, int BaseVersion);
}

