using System.Text.Json;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
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

public class EpicImportQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EpicImportQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IBlobSasService _sas;

    public EpicImportQueueWorker(
        IServiceProvider services,
        ILogger<EpicImportQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        IBlobSasService sas)
    {
        _services = services;
        _logger = logger;
        _sas = sas;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicImport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-import-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EpicImportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("EpicImportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EpicImportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
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
                    _logger.LogWarning("Received import message with null MessageId.");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                EpicImportQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<EpicImportQueuePayload>(message.MessageText);
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
                    var importService = scope.ServiceProvider.GetRequiredService<IEpicImportService>();
                    var queueService = scope.ServiceProvider.GetRequiredService<IEpicImportQueueService>();

                    var job = await db.EpicImportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is EpicImportJobStatus.Completed or EpicImportJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    await queueService.UpdateJobStatusAsync(job.Id, EpicImportJobStatus.Analyzing, stoppingToken);

                    try
                    {
                        _logger.LogInformation("Processing EpicImportJob: jobId={JobId} zipPath={ZipPath}", job.Id, job.ZipBlobPath);

                        var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath);
                        if (!await blobClient.ExistsAsync(stoppingToken))
                        {
                            _logger.LogWarning("Import ZIP not found for jobId={JobId} path={Path}", job.Id, job.ZipBlobPath);
                            await queueService.FailJobAsync(job.Id, "Import archive not found.", stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        await using var blobStream = await blobClient.OpenReadAsync(cancellationToken: stoppingToken);

                        // Extract manifest
                        var manifest = await importService.ExtractManifestAsync(blobStream, stoppingToken);

                        // Build import options from job
                        var options = new EpicImportRequest
                        {
                            ConflictStrategy = job.ConflictStrategy,
                            IncludeStories = job.IncludeStories,
                            IncludeHeroes = job.IncludeHeroes,
                            IncludeRegions = job.IncludeRegions,
                            IncludeImages = job.IncludeImages,
                            IncludeAudio = job.IncludeAudio,
                            IncludeVideo = job.IncludeVideo,
                            GenerateNewIds = job.GenerateNewIds,
                            IdPrefix = job.IdPrefix
                        };

                        // Generate ID mappings if needed
                        EpicIdMappings? idMappings = null;
                        if (options.GenerateNewIds || options.ConflictStrategy == "new_ids")
                        {
                            idMappings = await importService.GenerateIdMappingsAsync(manifest, options.IdPrefix, stoppingToken);
                        }

                        // Perform import
                        blobStream.Position = 0; // Reset stream
                        var result = await importService.ImportEpicAsync(
                            blobStream,
                            manifest,
                            options,
                            idMappings,
                            job.OwnerUserId,
                            job.Locale,
                            stoppingToken);

                        _logger.LogInformation("ImportEpicAsync completed: jobId={JobId} epicId={EpicId}", job.Id, result.ImportedEpicId);

                        // Complete job
                        await queueService.CompleteJobAsync(job.Id, result.ImportedEpicId, result.IdMappings, result.Warnings, stoppingToken);

                        // Clean up ZIP file
                        try
                        {
                            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: stoppingToken);
                        }
                        catch (Exception cleanupEx)
                        {
                            _logger.LogWarning(cleanupEx, "Failed to delete import archive for jobId={JobId}", job.Id);
                        }

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process import job: jobId={JobId}", job.Id);

                        if (job.DequeueCount >= 3)
                        {
                            await queueService.FailJobAsync(job.Id, ex.Message, stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicImportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicImportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in EpicImportQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in EpicImportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("EpicImportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private sealed record EpicImportQueuePayload(Guid JobId, string? OriginalEpicId = null);
}

