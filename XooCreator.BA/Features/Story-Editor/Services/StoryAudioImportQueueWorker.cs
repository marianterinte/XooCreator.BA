using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryAudioImportQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryAudioImportQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IBlobSasService _sas;
    private readonly IJobEventsHub _jobEvents;

    public StoryAudioImportQueueWorker(
        IServiceProvider services,
        ILogger<StoryAudioImportQueueWorker> logger,
        IConfiguration configuration,
        IBlobSasService sas,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _sas = sas;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AudioImport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-audio-import-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryAudioImportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryAudioImportQueueWorker started. QueueName={QueueName}", _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryAudioImportQueueWorker failed to create/connect to queue. Retrying in 30 seconds.");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromMinutes(10), cancellationToken: stoppingToken);

                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                if (message?.MessageId == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                var payload = DeserializePayload(message.MessageText);
                if (payload == null)
                {
                    _logger.LogError("Failed to deserialize audio import message");
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                var job = await db.StoryAudioImportJobs.FirstOrDefaultAsync(j => j.Id == payload.Value.JobId, stoppingToken);

                if (job == null || job.Status is StoryAudioImportJobStatus.Completed or StoryAudioImportJobStatus.Failed)
                {
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                _jobEvents.Publish(JobTypes.StoryAudioImport, job.Id, new { jobId = job.Id, storyId = job.StoryId, status = StoryAudioImportJobStatus.Running });

                job.Status = StoryAudioImportJobStatus.Running;
                job.StartedAtUtc = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);

                try
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IStoryAudioImportProcessor>();
                    if (!string.IsNullOrWhiteSpace(job.StagingPrefix))
                    {
                        var batchPayload = JsonSerializer.Deserialize<AudioBatchMappingPayload>(job.BatchMappingJson ?? string.Empty);
                        if (batchPayload == null || batchPayload.Files.Count == 0)
                        {
                            job.Status = StoryAudioImportJobStatus.Failed;
                            job.ErrorMessage = "Batch mapping payload is missing or invalid.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            _jobEvents.Publish(JobTypes.StoryAudioImport, job.Id, BuildPublishPayload(job));
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        var result = await processor.ProcessFromStagingAsync(batchPayload, job.StoryId, job.Locale, job.OwnerEmail, stoppingToken);
                        job.Success = result.Success;
                        job.ImportedCount = result.ImportedCount;
                        job.TotalPages = result.TotalPages;
                        job.ErrorsJson = result.Errors.Count > 0 ? JsonSerializer.Serialize(result.Errors) : null;
                        job.WarningsJson = result.Warnings.Count > 0 ? JsonSerializer.Serialize(result.Warnings) : null;
                        job.Status = StoryAudioImportJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = result.Errors.Count > 0 ? string.Join("; ", result.Errors) : null;

                        await DeleteStagingBlobsAsync(job.StagingPrefix, stoppingToken);
                    }
                    else
                    {
                        Stream? zipStream = null;
                        try
                        {
                            var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath ?? string.Empty);
                            if (!await blobClient.ExistsAsync(stoppingToken))
                            {
                                job.Status = StoryAudioImportJobStatus.Failed;
                                job.ErrorMessage = "ZIP file not found in storage.";
                                job.CompletedAtUtc = DateTime.UtcNow;
                                await db.SaveChangesAsync(stoppingToken);
                                _jobEvents.Publish(JobTypes.StoryAudioImport, job.Id, BuildPublishPayload(job));
                                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                                continue;
                            }

                            var download = await blobClient.DownloadStreamingAsync(cancellationToken: stoppingToken);
                            zipStream = download.Value.Content;
                            var result = await processor.ProcessAsync(zipStream, job.StoryId, job.Locale, job.OwnerEmail, stoppingToken);

                            job.Success = result.Success;
                            job.ImportedCount = result.ImportedCount;
                            job.TotalPages = result.TotalPages;
                            job.ErrorsJson = result.Errors.Count > 0 ? JsonSerializer.Serialize(result.Errors) : null;
                            job.WarningsJson = result.Warnings.Count > 0 ? JsonSerializer.Serialize(result.Warnings) : null;
                            job.Status = StoryAudioImportJobStatus.Completed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            job.ErrorMessage = result.Errors.Count > 0 ? string.Join("; ", result.Errors) : null;
                        }
                        finally
                        {
                            zipStream?.Dispose();
                        }

                        try
                        {
                            var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath ?? string.Empty);
                            await blobClient.DeleteIfExistsAsync(cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete temp import ZIP: {Path}", job.ZipBlobPath);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                    _jobEvents.Publish(JobTypes.StoryAudioImport, job.Id, BuildPublishPayload(job));
                    _logger.LogInformation("Completed StoryAudioImportJob: jobId={JobId} storyId={StoryId} imported={Count}", job.Id, job.StoryId, job.ImportedCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process StoryAudioImportJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
                    job.Status = StoryAudioImportJobStatus.Failed;
                    job.ErrorMessage = ex.Message;
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    _jobEvents.Publish(JobTypes.StoryAudioImport, job.Id, BuildPublishPayload(job));
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(job.StagingPrefix))
                        {
                            await DeleteStagingBlobsAsync(job.StagingPrefix, stoppingToken);
                        }
                        else if (!string.IsNullOrWhiteSpace(job.ZipBlobPath))
                        {
                            var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath);
                            await blobClient.DeleteIfExistsAsync(cancellationToken: stoppingToken);
                        }
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogWarning(deleteEx, "Failed to delete blob after failed audio import: {Path}", job.ZipBlobPath);
                    }
                }

                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryAudioImportQueueWorker loop. Retrying in 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryAudioImportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private static (Guid JobId, string StoryId)? DeserializePayload(string? messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText)) return null;
        try
        {
            var doc = JsonDocument.Parse(messageText);
            var root = doc.RootElement;
            if (root.TryGetProperty("JobId", out var jId) && root.TryGetProperty("StoryId", out var sId))
            {
                var jobId = Guid.Parse(jId.GetString() ?? "");
                var storyId = sId.GetString() ?? "";
                return (jobId, storyId);
            }
        }
        catch { }
        return null;
    }

    private static object BuildPublishPayload(StoryAudioImportJob job)
    {
        return new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            status = job.Status,
            queuedAtUtc = job.QueuedAtUtc,
            startedAtUtc = job.StartedAtUtc,
            completedAtUtc = job.CompletedAtUtc,
            errorMessage = job.ErrorMessage,
            success = job.Success,
            importedCount = job.ImportedCount,
            totalPages = job.TotalPages,
            errorsJson = job.ErrorsJson,
            warningsJson = job.WarningsJson
        };
    }

    private async Task DeleteStagingBlobsAsync(string stagingPrefix, CancellationToken ct)
    {
        var container = _sas.GetContainerClient(_sas.DraftContainer);
        await foreach (var blob in container.GetBlobsAsync(prefix: stagingPrefix.TrimEnd('/') + "/", cancellationToken: ct))
        {
            try
            {
                var client = container.GetBlobClient(blob.Name);
                await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed deleting staging blob {BlobName}", blob.Name);
            }
        }
    }
}
