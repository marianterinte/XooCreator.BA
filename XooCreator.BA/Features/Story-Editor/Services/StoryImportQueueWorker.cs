using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Features.StoryEditor.Endpoints;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryImportQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryImportQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IBlobSasService _sas;
    private readonly IJobEventsHub _jobEvents;

    public StoryImportQueueWorker(
        IServiceProvider services,
        ILogger<StoryImportQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        IBlobSasService sas,
        IJobEventsHub jobEvents)
    {
        _services = services;
        _logger = logger;
        _sas = sas;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Import"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-import-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryImportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryImportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryImportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromSeconds(60), stoppingToken);
                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                if (message?.MessageId == null)
                {
                    _logger.LogWarning("Received import message with null MessageId.");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                StoryImportQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryImportQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize import queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid payload for import messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var endpoint = scope.ServiceProvider.GetRequiredService<ImportFullStoryEndpoint>();

                    var job = await db.StoryImportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryImportJobStatus.Completed or StoryImportJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.StoryImport, job.Id, new
                        {
                            jobId = job.Id,
                            storyId = job.StoryId,
                            originalStoryId = job.OriginalStoryId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            importedAssets = job.ImportedAssets,
                            totalAssets = job.TotalAssets,
                            importedLanguagesCount = job.ImportedLanguagesCount,
                            errorMessage = job.ErrorMessage,
                            warningSummary = job.WarningSummary,
                            dequeueCount = job.DequeueCount
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryImportJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing StoryImportJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);

                        ImportFullStoryEndpoint.ImportJobResult result;
                        if (!string.IsNullOrEmpty(job.ManifestBlobPath) && !string.IsNullOrEmpty(job.StagingPrefix))
                        {
                            result = await endpoint.ProcessImportJobFromStagingAsync(job, stoppingToken);
                        }
                        else if (!string.IsNullOrEmpty(job.ZipBlobPath))
                        {
                            // Legacy: process whole ZIP from blob (request-upload + confirm-upload flow). Prefer client-side ZIP (ManifestBlobPath + StagingPrefix).
                            var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath);
                            if (!await blobClient.ExistsAsync(stoppingToken))
                            {
                                _logger.LogWarning("Import ZIP not found for jobId={JobId} path={Path}", job.Id, job.ZipBlobPath);
                                job.Status = StoryImportJobStatus.Failed;
                                job.ErrorMessage = "Import archive not found.";
                                job.CompletedAtUtc = DateTime.UtcNow;
                                await db.SaveChangesAsync(stoppingToken);
                                PublishStatus();
                                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                                continue;
                            }
                            await using var blobStream = await blobClient.OpenReadAsync(cancellationToken: stoppingToken);
                            result = await endpoint.ProcessImportJobAsync(job, blobStream, stoppingToken);

                            if (result.Success)
                            {
                                try
                                {
                                    await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: stoppingToken);
                                }
                                catch (Exception cleanupEx)
                                {
                                    _logger.LogWarning(cleanupEx, "Failed to delete import archive for jobId={JobId}", job.Id);
                                }
                            }
                        }
                        else
                        {
                            job.Status = StoryImportJobStatus.Failed;
                            job.ErrorMessage = "Job has no ZipBlobPath nor StagingPrefix.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }
                        _logger.LogInformation("ProcessImportJobAsync completed: jobId={JobId} success={Success} importedAssets={ImportedAssets}", job.Id, result.Success, result.UploadedAssets);

                        job.TotalAssets = result.TotalAssets;
                        job.ImportedAssets = result.UploadedAssets;
                        job.ImportedLanguagesCount = result.ImportedLanguages.Count;
                        job.WarningSummary = string.Join(Environment.NewLine, result.Warnings ?? Array.Empty<string>());
                        job.ErrorMessage = string.Join(Environment.NewLine, result.Errors ?? Array.Empty<string>());

                        if (result.Success)
                        {
                            job.Status = StoryImportJobStatus.Completed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            job.ErrorMessage = null;
                        }
                        else if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryImportJobStatus.Failed;
                            job.ErrorMessage = string.Join(Environment.NewLine, result.Errors ?? Array.Empty<string>());
                            job.CompletedAtUtc = DateTime.UtcNow;
                        }

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();

                        if (result.Success || job.Status == StoryImportJobStatus.Failed)
                        {
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process import job: jobId={JobId}", job.Id);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryImportJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        else
                        {
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryImportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryImportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in StoryImportQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryImportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryImportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private sealed record StoryImportQueuePayload(Guid JobId, string StoryId);
}

