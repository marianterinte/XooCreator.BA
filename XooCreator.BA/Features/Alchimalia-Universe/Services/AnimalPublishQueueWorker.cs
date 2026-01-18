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

public class AnimalPublishQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AnimalPublishQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public AnimalPublishQueueWorker(
        IServiceProvider services,
        ILogger<AnimalPublishQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AnimalPublish"] ?? "animal-publish-queue";
        _queueClient = queueClientFactory.CreateClient(queueName, "animal-publish-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AnimalPublishQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);
        
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("AnimalPublishQueueWorker started successfully. QueueName={QueueName}", _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AnimalPublishQueueWorker failed to create/connect to queue. QueueName={QueueName}.", _queueClient.Name);
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

                AnimalPublishQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<AnimalPublishQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.AnimalId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var service = scope.ServiceProvider.GetRequiredService<IAnimalCraftService>();

                    var job = await db.AnimalPublishJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status == AnimalPublishJobStatus.Completed || job.Status == AnimalPublishJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                         _jobEvents.Publish(JobTypes.AnimalPublish, job.Id, new
                        {
                            jobId = job.Id,
                            animalId = job.AnimalId,
                            status = job.Status
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = AnimalPublishJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing AnimalPublishJob: jobId={JobId} animalId={AnimalId}", job.Id, job.AnimalId);

                        // PublishAsync handles craft cleanup too
                        await service.PublishAsync(job.OwnerUserId, Guid.Parse(job.AnimalId), stoppingToken);

                        job.Status = AnimalPublishJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        _logger.LogInformation("AnimalPublishJob completed: jobId={JobId}", job.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process AnimalPublishJob: jobId={JobId} animalId={AnimalId}", job.Id, job.AnimalId);

                        if (job.DequeueCount >= 3)
                        {
                            // Clear the ChangeTracker to remove any invalid state from the failed PublishAsync call
                            db.ChangeTracker.Clear();
                            
                            var jobToFail = await db.AnimalPublishJobs.FirstOrDefaultAsync(j => j.Id == job.Id, stoppingToken);
                            if (jobToFail != null)
                            {
                                jobToFail.Status = AnimalPublishJobStatus.Failed;
                                jobToFail.ErrorMessage = ex.Message;
                                jobToFail.CompletedAtUtc = DateTime.UtcNow;
                                jobToFail.DequeueCount = job.DequeueCount;
                                
                                await db.SaveChangesAsync(stoppingToken);

                                _jobEvents.Publish("AnimalPublish", jobToFail.Id, new
                                {
                                    jobId = jobToFail.Id,
                                    animalId = jobToFail.AnimalId,
                                    status = jobToFail.Status
                                });
                                
                                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AnimalPublishQueueWorker loop.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private sealed record AnimalPublishQueuePayload(Guid JobId, string AnimalId);
}
