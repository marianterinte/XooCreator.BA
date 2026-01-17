using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class HeroPublishQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<HeroPublishQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public HeroPublishQueueWorker(
        IServiceProvider services,
        ILogger<HeroPublishQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["HeroPublish"] ?? "hero-publish-queue";
        _queueClient = queueClientFactory.CreateClient(queueName, "hero-publish-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HeroPublishQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);
        
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("HeroPublishQueueWorker started successfully. QueueName={QueueName}", _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HeroPublishQueueWorker failed to create/connect to queue. QueueName={QueueName}.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromSeconds(60), cancellationToken: stoppingToken);
                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                if (message?.MessageId == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                HeroPublishQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<HeroPublishQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.HeroId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var service = scope.ServiceProvider.GetRequiredService<IHeroDefinitionCraftService>();

                    var job = await db.HeroPublishJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status == HeroPublishJobStatus.Completed || job.Status == HeroPublishJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        // TODO: Add generic JobType or specific HeroPublish type to JobTypes
                        // For now using raw string or existing generic if available
                        _jobEvents.Publish("HeroPublish", job.Id, new
                        {
                            jobId = job.Id,
                            heroId = job.HeroId,
                            status = job.Status
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = HeroPublishJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing HeroPublishJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);

                        // The service now handles publishing AND deleting the draft
                        await service.PublishAsync(job.OwnerUserId, job.HeroId, stoppingToken);

                        job.Status = HeroPublishJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        _logger.LogInformation("HeroPublishJob completed: jobId={JobId}", job.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process HeroPublishJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = HeroPublishJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HeroPublishQueueWorker loop.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private sealed record HeroPublishQueuePayload(Guid JobId, string HeroId);
}
