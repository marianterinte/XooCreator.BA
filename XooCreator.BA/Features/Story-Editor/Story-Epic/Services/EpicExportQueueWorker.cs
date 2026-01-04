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
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicExportQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EpicExportQueueWorker> _logger;
    private readonly QueueClient _queueClient;

    public EpicExportQueueWorker(
        IServiceProvider services,
        ILogger<EpicExportQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-export-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EpicExportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("EpicExportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EpicExportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(5), stoppingToken);
                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                if (message?.MessageId == null)
                {
                    _logger.LogWarning("Received export message with null MessageId.");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                EpicExportQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<EpicExportQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize export queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.EpicId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                // Create scope for each message to ensure proper disposal of scoped services
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var exportService = scope.ServiceProvider.GetRequiredService<IEpicExportService>();
                    var queueService = scope.ServiceProvider.GetRequiredService<IEpicExportQueueService>();
                    var sas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();

                    var job = await db.EpicExportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is EpicExportJobStatus.Completed or EpicExportJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = EpicExportJobStatus.Preparing;
                    await db.SaveChangesAsync(stoppingToken);

                    try
                    {
                        _logger.LogInformation("Processing EpicExportJob: jobId={JobId} epicId={EpicId} isDraft={IsDraft}",
                            job.Id, job.EpicId, job.IsDraft);

                        // Build export options
                        var options = new EpicExportRequest
                        {
                            IncludeStories = job.IncludeStories,
                            IncludeHeroes = job.IncludeHeroes,
                            IncludeRegions = job.IncludeRegions,
                            IncludeImages = job.IncludeImages,
                            IncludeAudio = job.IncludeAudio,
                            IncludeVideo = job.IncludeVideo,
                            IncludeProgress = job.IncludeProgress,
                            LanguageFilter = job.LanguageFilter
                        };

                        // Build manifest
                        await queueService.UpdateJobProgressAsync(job.Id, EpicExportJobStatus.Preparing, "Preparing", null, stoppingToken);
                        var manifest = await exportService.BuildManifestAsync(job.EpicId, options, job.IsDraft, stoppingToken);

                        // Update progress with counts
                        await queueService.UpdateJobProgressAsync(job.Id, EpicExportJobStatus.PackagingStories, "PackagingStories", 
                            j => {
                                j.TotalStories = manifest.Stories.Count;
                                j.TotalHeroes = manifest.Heroes.Count;
                                j.TotalRegions = manifest.Regions.Count;
                            }, stoppingToken);

                        // Create ZIP
                        await queueService.UpdateJobProgressAsync(job.Id, EpicExportJobStatus.PackagingStories, "CreatingZIP", null, stoppingToken);
                        await using var zipStream = await exportService.CreateZipArchiveAsync(manifest, job.EpicId, job.IsDraft, stoppingToken);

                        // Upload to blob storage
                        await queueService.UpdateJobProgressAsync(job.Id, EpicExportJobStatus.Finalizing, "Uploading", null, stoppingToken);
                        var fileName = job.IsDraft 
                            ? $"{job.EpicId}-draft-export.zip"
                            : $"{job.EpicId}-v{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
                        
                        var uploadResult = await exportService.UploadExportAsync(zipStream, job.EpicId, fileName, stoppingToken);

                        // Mark complete
                        await queueService.CompleteJobAsync(job.Id, uploadResult.BlobPath, uploadResult.BlobPath.Split('/').Last(), uploadResult.SizeBytes, stoppingToken);

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("Successfully completed EpicExportJob: jobId={JobId} epicId={EpicId} zipSizeBytes={ZipSizeBytes}",
                            job.Id, job.EpicId, uploadResult.SizeBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process EpicExportJob: jobId={JobId} epicId={EpicId}",
                            job.Id, job.EpicId);

                        if (job.DequeueCount >= 3)
                        {
                            await queueService.FailJobAsync(job.Id, ex.Message, stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        // If DequeueCount < 3, don't delete message - it will be retried
                    }
                } // Scope disposed here
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicExportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicExportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in EpicExportQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in EpicExportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("EpicExportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }
}

internal record EpicExportQueuePayload
{
    public Guid JobId { get; init; }
    public string EpicId { get; init; } = string.Empty;
    public bool IsDraft { get; init; }
}

