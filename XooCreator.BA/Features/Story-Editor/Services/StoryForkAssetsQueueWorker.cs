using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Endpoints;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryForkAssetsQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryForkAssetsQueueWorker> _logger;
    private readonly QueueClient _queueClient;

    public StoryForkAssetsQueueWorker(
        IServiceProvider services,
        ILogger<StoryForkAssetsQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["ForkAssets"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-fork-assets-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryForkAssetsQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryForkAssetsQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryForkAssetsQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
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
                    _logger.LogWarning("Received fork assets message with null MessageId.");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                StoryForkAssetsQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryForkAssetsQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize fork queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid payload for fork messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var endpoint = scope.ServiceProvider.GetRequiredService<ForkStoryEndpoint>();

                    var job = await db.StoryForkAssetJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryForkAssetJobStatus.Completed or StoryForkAssetJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryForkAssetJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);

                    try
                    {
                        _logger.LogInformation("Processing StoryForkAssetJob: jobId={JobId}", job.Id);
                        var result = await endpoint.ProcessForkAssetJobAsync(job, stoppingToken);
                        _logger.LogInformation("ProcessForkAssetJobAsync completed: jobId={JobId} success={Success} copied={CopiedAssets}", job.Id, result.Success, result.CopiedAssets);

                        job.AttemptedAssets = result.AttemptedAssets;
                        job.CopiedAssets = result.CopiedAssets;
                        job.WarningSummary = string.Join(Environment.NewLine, result.Warnings ?? Array.Empty<string>());
                        job.ErrorMessage = string.Join(Environment.NewLine, result.Errors ?? Array.Empty<string>());

                        if (result.Success)
                        {
                            job.Status = StoryForkAssetJobStatus.Completed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await UpdateParentForkJobStatusAsync(db, job, stoppingToken);
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        else if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryForkAssetJobStatus.Failed;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await UpdateParentForkJobStatusAsync(db, job, stoppingToken);
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        else
                        {
                            await UpdateParentForkJobStatusAsync(db, job, stoppingToken);
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process fork asset job: jobId={JobId}", job.Id);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryForkAssetJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await UpdateParentForkJobStatusAsync(db, job, stoppingToken);
                            await db.SaveChangesAsync(stoppingToken);
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                        else
                        {
                            await UpdateParentForkJobStatusAsync(db, job, stoppingToken);
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryForkAssetsQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryForkAssetsQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in StoryForkAssetsQueueWorker. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryForkAssetsQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryForkAssetsQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private sealed record StoryForkAssetsQueuePayload(Guid JobId, string TargetStoryId);

    private static async Task UpdateParentForkJobStatusAsync(
        XooDbContext db,
        StoryForkAssetJob assetJob,
        CancellationToken ct)
    {
        var parentJob = await db.StoryForkJobs.FirstOrDefaultAsync(j => j.AssetJobId == assetJob.Id, ct);
        if (parentJob == null)
        {
            return;
        }

        parentJob.AssetJobId ??= assetJob.Id;
        parentJob.AssetJobStatus = assetJob.Status;
    }
}

