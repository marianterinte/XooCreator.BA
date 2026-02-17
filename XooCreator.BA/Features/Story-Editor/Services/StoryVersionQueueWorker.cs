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
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryVersionQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryVersionQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public StoryVersionQueueWorker(
        IServiceProvider services,
        ILogger<StoryVersionQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Version"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-version-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryVersionQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryVersionQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}", 
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryVersionQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
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

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.StoryVersion, job.Id, new
                        {
                            jobId = job.Id,
                            storyId = job.StoryId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            dequeueCount = job.DequeueCount,
                            baseVersion = job.BaseVersion
                        });
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
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryVersionJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        // Load published StoryDefinition with all necessary includes
                        var definition = await db.StoryDefinitions
                            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
                            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
                            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
                            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
                            .Include(d => d.Translations)
                            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
                            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
                            .Include(d => d.DialogParticipants)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(d => d.StoryId == job.StoryId, stoppingToken);

                        if (definition == null)
                        {
                            job.Status = StoryVersionJobStatus.Failed;
                            job.ErrorMessage = "StoryDefinition not found.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        if (definition.Status != StoryStatus.Published)
                        {
                            job.Status = StoryVersionJobStatus.Failed;
                            job.ErrorMessage = $"Story is not published (status: {definition.Status}).";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        if (definition.Version != job.BaseVersion)
                        {
                            job.Status = StoryVersionJobStatus.Superseded;
                            job.ErrorMessage = $"Story version changed from {job.BaseVersion} to {definition.Version}.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
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
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        _logger.LogInformation("Processing StoryVersionJob: jobId={JobId} storyId={StoryId} baseVersion={BaseVersion}",
                            job.Id, job.StoryId, job.BaseVersion);

                        // Create StoryCraft from StoryDefinition
                        var newCraft = await storyCopyService.CreateCopyFromDefinitionAsync(definition, job.OwnerUserId, job.StoryId, stoppingToken);
                        _logger.LogInformation("Draft created from definition for StoryVersionJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);

                        // Copy assets from published to draft storage.
                        // Source: published content is under the owner folder. Target: draft must be under the same
                        // owner (draft/u/{owner}/stories/...) so the editor's request-read finds the blobs;
                        // the editor always uses craft.OwnerUserId → owner email for draft paths.
                        try
                        {
                            var publishedOwnerEmail =
                                TryExtractOwnerFolderFromPublishedPath(definition.CoverImageUrl, definition.StoryId)
                                ?? TryExtractOwnerFolderFromPublishedPath(
                                    definition.Tiles.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.ImageUrl))?.ImageUrl,
                                    definition.StoryId)
                                ?? await db.AlchimaliaUsers
                                    .AsNoTracking()
                                    .Where(u => u.Id == job.OwnerUserId)
                                    .Select(u => u.Email)
                                    .FirstOrDefaultAsync(stoppingToken);

                            if (string.IsNullOrWhiteSpace(publishedOwnerEmail))
                            {
                                _logger.LogWarning(
                                    "Skipping asset copy for StoryVersionJob: jobId={JobId} storyId={StoryId} — cannot resolve published owner email for OwnerUserId={OwnerUserId}",
                                    job.Id, job.StoryId, job.OwnerUserId);
                            }
                            else
                            {
                                var assets = storyAssetCopyService.CollectFromDefinition(definition);
                                await storyAssetCopyService.CopyPublishedToDraftAsync(
                                    assets,
                                    publishedOwnerEmail,
                                    definition.StoryId,
                                    publishedOwnerEmail,
                                    job.StoryId,
                                    stoppingToken);
                                _logger.LogInformation("Asset copy completed for StoryVersionJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
                            }
                        }
                        catch (Exception assetEx)
                        {
                            _logger.LogWarning(assetEx, "Failed to copy assets during create version for storyId={StoryId}, but continuing", job.StoryId);
                        }

                        job.Status = StoryVersionJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();

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
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        // If DequeueCount < 3, don't delete message - queue will re-deliver it
                    }
                } // Scope disposed here - DbContext and all scoped services are cleaned up
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryVersionQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryVersionQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in StoryVersionQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryVersionQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryVersionQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record StoryVersionQueuePayload(Guid JobId, string StoryId, int BaseVersion);

    private static string? TryExtractOwnerFolderFromPublishedPath(string? path, string storyId)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var clean = path.Trim();
        if (Uri.TryCreate(clean, UriKind.Absolute, out var uri))
        {
            clean = uri.AbsolutePath.TrimStart('/');
        }
        else
        {
            clean = clean.TrimStart('/');
        }

        const string marker = "/stories/";
        var idx = clean.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return null;
        }

        var after = clean[(idx + marker.Length)..];
        var parts = after.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 1)
        {
            return null;
        }

        var owner = parts[0];
        if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(storyId))
        {
            var storySegment = parts[1];
            if (!storySegment.Equals(storyId, StringComparison.OrdinalIgnoreCase))
            {
                return owner;
            }
        }

        return owner;
    }
}

