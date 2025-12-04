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

    public StoryImportQueueWorker(
        IServiceProvider services,
        ILogger<StoryImportQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        IBlobSasService sas)
    {
        _services = services;
        _logger = logger;
        _sas = sas;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Import"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-import-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        _logger.LogInformation("StoryImportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
            _queueClient.Name, _queueClient.Uri);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromSeconds(60), stoppingToken);
                if (messages.Value == null || messages.Value.Length == 0)
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

                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                var endpoint = scope.ServiceProvider.GetRequiredService<ImportFullStoryEndpoint>();

                var job = await db.StoryImportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                if (job == null || job.Status is StoryImportJobStatus.Completed or StoryImportJobStatus.Failed)
                {
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                job.DequeueCount += 1;
                job.StartedAtUtc ??= DateTime.UtcNow;
                job.Status = StoryImportJobStatus.Running;
                await db.SaveChangesAsync(stoppingToken);

                try
                {
                    var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath);
                    if (!await blobClient.ExistsAsync(stoppingToken))
                    {
                        _logger.LogWarning("Import ZIP not found for jobId={JobId} path={Path}", job.Id, job.ZipBlobPath);
                        job.Status = StoryImportJobStatus.Failed;
                        job.ErrorMessage = "Import archive not found.";
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    await using var blobStream = await blobClient.OpenReadAsync(cancellationToken: stoppingToken);

                    var result = await endpoint.ProcessImportJobAsync(job, blobStream, stoppingToken);

                    job.TotalAssets = result.TotalAssets;
                    job.ImportedAssets = result.UploadedAssets;
                    job.ImportedLanguagesCount = result.ImportedLanguages.Count;
                    job.WarningSummary = string.Join(Environment.NewLine, result.Warnings);
                    job.ErrorMessage = string.Join(Environment.NewLine, result.Errors);

                    if (result.Success)
                    {
                        job.Status = StoryImportJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;

                        try
                        {
                            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: stoppingToken);
                        }
                        catch (Exception cleanupEx)
                        {
                            _logger.LogWarning(cleanupEx, "Failed to delete import archive for jobId={JobId}", job.Id);
                        }
                    }
                    else
                    {
                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryImportJobStatus.Failed;
                            job.ErrorMessage = string.Join(Environment.NewLine, result.Errors);
                            job.CompletedAtUtc = DateTime.UtcNow;
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);

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
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    else
                    {
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryImportQueueWorker loop.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private sealed record StoryImportQueuePayload(Guid JobId, string StoryId);
}

