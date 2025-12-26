using System.Text.Json;
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

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryDocumentExportQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryDocumentExportQueueWorker> _logger;
    private readonly QueueClient _queueClient;

    public StoryDocumentExportQueueWorker(
        IServiceProvider services,
        ILogger<StoryDocumentExportQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["DocumentExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-document-export-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryDocumentExportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryDocumentExportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryDocumentExportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: TimeSpan.FromSeconds(60),
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

                StoryDocumentExportQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryDocumentExportQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize document export queue message: messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.StoryId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                var exporter = scope.ServiceProvider.GetRequiredService<IStoryDocumentExportService>();
                var sas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();

                var job = await db.StoryDocumentExportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                if (job == null || job.Status is StoryDocumentExportJobStatus.Completed or StoryDocumentExportJobStatus.Failed)
                {
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                job.DequeueCount += 1;
                job.StartedAtUtc ??= DateTime.UtcNow;
                job.Status = StoryDocumentExportJobStatus.Running;
                await db.SaveChangesAsync(stoppingToken);

                try
                {
                    _logger.LogInformation("Processing StoryDocumentExportJob: jobId={JobId} storyId={StoryId} isDraft={IsDraft} format={Format}",
                        job.Id, job.StoryId, job.IsDraft, job.Format);

                    var result = await exporter.ExportAsync(job, stoppingToken);

                    var ext = Path.GetExtension(result.FileName);
                    if (string.IsNullOrWhiteSpace(ext))
                    {
                        ext = job.Format == StoryDocumentExportFormat.Docx ? ".docx" : ".pdf";
                    }

                    var blobPath = $"exports-documents/{job.Id}/{result.FileName}".Replace('\\', '/');
                    var blobClient = sas.GetBlobClient(sas.DraftContainer, blobPath);
                    await blobClient.UploadAsync(new BinaryData(result.Bytes), overwrite: true, stoppingToken);

                    job.Status = StoryDocumentExportJobStatus.Completed;
                    job.CompletedAtUtc = DateTime.UtcNow;
                    job.ErrorMessage = null;
                    job.OutputBlobPath = blobPath;
                    job.OutputFileName = result.FileName;
                    job.OutputSizeBytes = result.SizeBytes;

                    await db.SaveChangesAsync(stoppingToken);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                    _logger.LogInformation("Successfully completed StoryDocumentExportJob: jobId={JobId} storyId={StoryId} sizeBytes={SizeBytes}",
                        job.Id, job.StoryId, result.SizeBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process StoryDocumentExportJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);

                    if (job.DequeueCount >= 3)
                    {
                        job.Status = StoryDocumentExportJobStatus.Failed;
                        job.ErrorMessage = ex.Message;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    // If DequeueCount < 3, don't delete message - it will be retried
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryDocumentExportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryDocumentExportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in StoryDocumentExportQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryDocumentExportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryDocumentExportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private sealed record StoryDocumentExportQueuePayload(Guid JobId, string StoryId);
}


