using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryExportQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryExportQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;
    private readonly TimeSpan _messageVisibilityTimeout;

    public StoryExportQueueWorker(
        IServiceProvider services,
        ILogger<StoryExportQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Export"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-export-full-export-queue");
        var visibilityTimeoutSeconds = configuration.GetValue<int?>("AzureStorage:Queues:ExportVisibilityTimeoutSeconds");
        _messageVisibilityTimeout = TimeSpan.FromSeconds(
            visibilityTimeoutSeconds.HasValue && visibilityTimeoutSeconds.Value > 0
                ? visibilityTimeoutSeconds.Value
                : 900); // 15 minutes default for heavy dialog exports
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryExportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryExportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryExportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: _messageVisibilityTimeout,
                    cancellationToken: stoppingToken);

                if (messages?.Value == null || messages.Value.Length == 0)
                {
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

                _logger.LogInformation("Received export message from queue: messageId={MessageId} queueName={QueueName}",
                    message.MessageId, _queueClient.Name);

                StoryExportQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryExportQueuePayload>(message.MessageText);
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

                // Create scope for each message to ensure proper disposal of scoped services
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var exportService = scope.ServiceProvider.GetRequiredService<IStoryExportService>();
                    var sas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();
                    var crafts = scope.ServiceProvider.GetRequiredService<IStoryCraftsRepository>();

                    var job = await db.StoryExportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryExportJobStatus.Completed or StoryExportJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    // Another instance may already be processing this job when visibility timeout expires.
                    // Keep message for retry later instead of processing the same job concurrently.
                    if (job.Status == StoryExportJobStatus.Running)
                    {
                        _logger.LogInformation(
                            "Skipping export message for already running job: jobId={JobId} storyId={StoryId} dequeueCount={DequeueCount}",
                            job.Id, job.StoryId, job.DequeueCount);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.StoryExport, job.Id, new
                        {
                            jobId = job.Id,
                            storyId = job.StoryId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            zipDownloadUrl = (string?)null,
                            zipFileName = job.ZipFileName,
                            zipSizeBytes = job.ZipSizeBytes,
                            mediaCount = job.MediaCount,
                            languageCount = job.LanguageCount
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryExportJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing StoryExportJob: jobId={JobId} storyId={StoryId} isDraft={IsDraft}",
                            job.Id, job.StoryId, job.IsDraft);

                        ExportResult exportResult;
                        
                        // Process export based on type (draft or published)
                        if (job.IsDraft)
                        {
                            exportResult = await ProcessDraftExportAsync(job, exportService, crafts, db, stoppingToken);
                        }
                        else
                        {
                            exportResult = await ProcessPublishedExportAsync(job, exportService, db, stoppingToken);
                        }

                        // Save ZIP to blob storage
                        var zipBlobPath = $"exports/{job.Id}/{exportResult.FileName}";
                        var blobClient = sas.GetBlobClient(sas.DraftContainer, zipBlobPath); // Use draft container for exports
                        await blobClient.UploadAsync(new BinaryData(exportResult.ZipBytes), overwrite: true, stoppingToken);

                        // Update job with results
                        job.Status = StoryExportJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        job.ZipBlobPath = zipBlobPath;
                        job.ZipFileName = exportResult.FileName;
                        job.ZipSizeBytes = exportResult.ZipSizeBytes;
                        job.MediaCount = exportResult.MediaCount;
                        job.LanguageCount = exportResult.LanguageCount;

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("Successfully completed StoryExportJob: jobId={JobId} storyId={StoryId} zipSizeBytes={ZipSizeBytes}",
                            job.Id, job.StoryId, exportResult.ZipSizeBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process StoryExportJob: jobId={JobId} storyId={StoryId}",
                            job.Id, job.StoryId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryExportJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        // If DequeueCount < 3, don't delete message - it will be retried
                    }
                } // Scope disposed here
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryExportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryExportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in StoryExportQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryExportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryExportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private async Task<ExportResult> ProcessDraftExportAsync(
        StoryExportJob job,
        IStoryExportService exportService,
        IStoryCraftsRepository crafts,
        XooDbContext db,
        CancellationToken ct)
    {
        var craft = await crafts.GetAsync(job.StoryId, ct);
        if (craft == null)
        {
            throw new InvalidOperationException($"StoryCraft not found: {job.StoryId}");
        }

        // Resolve owner email
        var owner = await db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => u.Id == job.OwnerUserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new InvalidOperationException($"Owner email not found for userId: {job.OwnerUserId}");
        }

        return await exportService.ExportDraftStoryAsync(craft, job.Locale, owner, ct);
    }

    private async Task<ExportResult> ProcessPublishedExportAsync(
        StoryExportJob job,
        IStoryExportService exportService,
        XooDbContext db,
        CancellationToken ct)
    {
        var def = await db.StoryDefinitions
            .AsNoTracking()
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
            .Include(d => d.DialogParticipants)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.StoryId == job.StoryId, ct);

        if (def == null)
        {
            throw new InvalidOperationException($"StoryDefinition not found: {job.StoryId}");
        }

        return await exportService.ExportPublishedStoryAsync(def, job.Locale, ct);
    }

    private sealed record StoryExportQueuePayload(Guid JobId, string StoryId, bool IsDraft);
}
