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

public class StoryForkQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryForkQueueWorker> _logger;
    private readonly QueueClient _queueClient;

    public StoryForkQueueWorker(
        IServiceProvider services,
        ILogger<StoryForkQueueWorker> logger,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Fork"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-fork-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        _logger.LogInformation("StoryForkQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
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
                    _logger.LogWarning("Received fork message with null MessageId.");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                StoryForkQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryForkQueuePayload>(message.MessageText);
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

                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                var endpoint = scope.ServiceProvider.GetRequiredService<ForkStoryEndpoint>();

                var job = await db.StoryForkJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                if (job == null || job.Status is StoryForkJobStatus.Completed or StoryForkJobStatus.Failed)
                {
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                job.DequeueCount += 1;
                job.StartedAtUtc ??= DateTime.UtcNow;
                job.Status = StoryForkJobStatus.Running;
                await db.SaveChangesAsync(stoppingToken);

                try
                {
                    var result = await endpoint.ProcessForkJobAsync(job, stoppingToken);

                    job.WarningSummary = string.Join(Environment.NewLine, result.Warnings);
                    job.ErrorMessage = string.Join(Environment.NewLine, result.Errors);
                    job.AssetJobId = result.AssetJobId;
                    job.AssetJobStatus = result.AssetJobStatus;
                    job.SourceTiles = result.SourceTiles;
                    job.SourceTranslations = result.SourceTranslations;

                    if (result.Success)
                    {
                        job.Status = StoryForkJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    else
                    {
                        if (job.DequeueCount >= 3)
                        {
                            job.Status = StoryForkJobStatus.Failed;
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process fork job: jobId={JobId}", job.Id);

                    if (job.DequeueCount >= 3)
                    {
                        job.Status = StoryForkJobStatus.Failed;
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
                _logger.LogError(ex, "Unexpected error in StoryForkQueueWorker loop.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private sealed record StoryForkQueuePayload(Guid JobId, string TargetStoryId);
}

