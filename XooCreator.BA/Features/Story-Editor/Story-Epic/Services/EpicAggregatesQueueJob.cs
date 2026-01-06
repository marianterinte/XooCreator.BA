using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicAggregatesQueueJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EpicAggregatesQueueJob> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public EpicAggregatesQueueJob(
        IServiceProvider services,
        ILogger<EpicAggregatesQueueJob> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicAggregates"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-aggregates-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EpicAggregatesQueueJob initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("EpicAggregatesQueueJob started. QueueName={QueueName} QueueUri={QueueUri}", 
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EpicAggregatesQueueJob failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromSeconds(60), cancellationToken: stoppingToken);
                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    // Log only every 10th check to avoid spam (every ~30 seconds)
                    if (DateTime.UtcNow.Second % 30 < 3)
                    {
                        _logger.LogDebug("No messages in queue. QueueName={QueueName}", _queueClient.Name);
                    }
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

                _logger.LogInformation("Received message from queue: messageId={MessageId} queueName={QueueName}", 
                    message.MessageId, _queueClient.Name);

                EpicAggregatesQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<EpicAggregatesQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.JobType))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                // Process based on JobType
                if (payload.JobType == "HeroVersion")
                {
                    await ProcessHeroVersionJobAsync(payload, message, stoppingToken);
                }
                else if (payload.JobType == "RegionVersion")
                {
                    await ProcessRegionVersionJobAsync(payload, message, stoppingToken);
                }
                else
                {
                    _logger.LogWarning("Unknown job type: {JobType} for messageId={MessageId}", payload.JobType, message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicAggregatesQueueJob stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EpicAggregatesQueueJob stopping due to task cancellation.");
                break;
            }
            catch (Azure.RequestFailedException azureEx)
            {
                _logger.LogError(azureEx, "Azure Storage error in EpicAggregatesQueueJob. Status={Status} ErrorCode={ErrorCode}. Retrying in 10 seconds.",
                    azureEx.Status, azureEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in EpicAggregatesQueueJob loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("EpicAggregatesQueueJob has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private async Task ProcessHeroVersionJobAsync(EpicAggregatesQueuePayload payload, QueueMessage message, CancellationToken stoppingToken)
    {
        // Create scope for each message to ensure proper disposal of scoped services (DbContext, etc.)
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
            var heroService = scope.ServiceProvider.GetRequiredService<IEpicHeroService>();

            var job = await db.HeroVersionJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
            if (job == null || job.Status is HeroVersionJobStatus.Completed or HeroVersionJobStatus.Failed or HeroVersionJobStatus.Superseded)
            {
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                return;
            }

            void PublishStatus()
            {
                _jobEvents.Publish(JobTypes.HeroVersion, job.Id, new
                {
                    jobId = job.Id,
                    heroId = job.HeroId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                });
            }

            // Check if there's another running job for the same hero
            var runningJob = await db.HeroVersionJobs
                .FirstOrDefaultAsync(j => j.HeroId == job.HeroId && j.Id != job.Id && j.Status == HeroVersionJobStatus.Running, stoppingToken);
            if (runningJob != null)
            {
                job.Status = HeroVersionJobStatus.Superseded;
                job.ErrorMessage = $"Job superseded by running job {runningJob.Id}.";
                job.CompletedAtUtc = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);
                PublishStatus();
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                return;
            }

            job.DequeueCount += 1;
            job.StartedAtUtc ??= DateTime.UtcNow;
            job.Status = HeroVersionJobStatus.Running;
            await db.SaveChangesAsync(stoppingToken);
            PublishStatus();

            try
            {
                // Load published EpicHeroDefinition with all necessary includes
                var definition = await db.EpicHeroDefinitions
                    .Include(d => d.Translations)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(d => d.Id == job.HeroId, stoppingToken);

                if (definition == null)
                {
                    job.Status = HeroVersionJobStatus.Failed;
                    job.ErrorMessage = "EpicHeroDefinition not found.";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                if (definition.Status != "published")
                {
                    job.Status = HeroVersionJobStatus.Failed;
                    job.ErrorMessage = $"Hero is not published (status: {definition.Status}).";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                if (definition.Version != job.BaseVersion)
                {
                    job.Status = HeroVersionJobStatus.Superseded;
                    job.ErrorMessage = $"Hero version changed from {job.BaseVersion} to {definition.Version}.";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                // Check if draft already exists (use AsNoTracking to avoid tracking conflicts)
                var existingCraft = await db.EpicHeroCrafts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == job.HeroId, stoppingToken);
                if (existingCraft != null && existingCraft.Status != "published")
                {
                    job.Status = HeroVersionJobStatus.Failed;
                    job.ErrorMessage = "A draft already exists for this hero. Please edit or publish it first.";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                _logger.LogInformation("Processing HeroVersionJob: jobId={JobId} heroId={HeroId} baseVersion={BaseVersion}",
                    job.Id, job.HeroId, job.BaseVersion);

                try
                {
                    await heroService.CreateVersionFromPublishedAsync(job.OwnerUserId, job.HeroId, stoppingToken);
                    _logger.LogInformation("CreateVersionFromPublishedAsync (hero) completed: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);
                }
                catch (Exception versionEx)
                {
                    _logger.LogError(versionEx, "CreateVersionFromPublishedAsync (hero) failed: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);
                    throw;
                }

                job.Status = HeroVersionJobStatus.Completed;
                job.CompletedAtUtc = DateTime.UtcNow;
                job.ErrorMessage = null;
                
                await db.SaveChangesAsync(stoppingToken);
                PublishStatus();

                _logger.LogInformation("HeroVersionJob completed: jobId={JobId} heroId={HeroId} baseVersion={BaseVersion}",
                    job.Id, job.HeroId, job.BaseVersion);

                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process HeroVersionJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);

                if (job.DequeueCount >= 3)
                {
                    job.Status = HeroVersionJobStatus.Failed;
                    job.ErrorMessage = ex.Message;
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                }
                // If DequeueCount < 3, don't delete message - queue will re-deliver it
            }
        } // Scope disposed here - DbContext and all scoped services are cleaned up
    }

    private async Task ProcessRegionVersionJobAsync(EpicAggregatesQueuePayload payload, QueueMessage message, CancellationToken stoppingToken)
    {
        // Create scope for each message to ensure proper disposal of scoped services (DbContext, etc.)
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
            var regionService = scope.ServiceProvider.GetRequiredService<IStoryRegionService>();

            var job = await db.RegionVersionJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
            if (job == null || job.Status is RegionVersionJobStatus.Completed or RegionVersionJobStatus.Failed or RegionVersionJobStatus.Superseded)
            {
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                return;
            }

            void PublishStatus()
            {
                _jobEvents.Publish(JobTypes.RegionVersion, job.Id, new
                {
                    jobId = job.Id,
                    regionId = job.RegionId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                });
            }

            // Check if there's another running job for the same region
            var runningJob = await db.RegionVersionJobs
                .FirstOrDefaultAsync(j => j.RegionId == job.RegionId && j.Id != job.Id && j.Status == RegionVersionJobStatus.Running, stoppingToken);
            if (runningJob != null)
            {
                job.Status = RegionVersionJobStatus.Superseded;
                job.ErrorMessage = $"Job superseded by running job {runningJob.Id}.";
                job.CompletedAtUtc = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);
                PublishStatus();
                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                return;
            }

            job.DequeueCount += 1;
            job.StartedAtUtc ??= DateTime.UtcNow;
            job.Status = RegionVersionJobStatus.Running;
            await db.SaveChangesAsync(stoppingToken);
            PublishStatus();

            try
            {
                // Load published StoryRegionDefinition with all necessary includes
                var definition = await db.StoryRegionDefinitions
                    .Include(d => d.Translations)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(d => d.Id == job.RegionId, stoppingToken);

                if (definition == null)
                {
                    job.Status = RegionVersionJobStatus.Failed;
                    job.ErrorMessage = "StoryRegionDefinition not found.";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                if (definition.Status != "published")
                {
                    job.Status = RegionVersionJobStatus.Failed;
                    job.ErrorMessage = $"Region is not published (status: {definition.Status}).";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                if (definition.Version != job.BaseVersion)
                {
                    job.Status = RegionVersionJobStatus.Superseded;
                    job.ErrorMessage = $"Region version changed from {job.BaseVersion} to {definition.Version}.";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                // Check if draft already exists (use AsNoTracking to avoid tracking conflicts)
                var existingCraft = await db.StoryRegionCrafts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == job.RegionId, stoppingToken);
                if (existingCraft != null && existingCraft.Status != "published")
                {
                    job.Status = RegionVersionJobStatus.Failed;
                    job.ErrorMessage = "A draft already exists for this region. Please edit or publish it first.";
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    return;
                }

                _logger.LogInformation("Processing RegionVersionJob: jobId={JobId} regionId={RegionId} baseVersion={BaseVersion}",
                    job.Id, job.RegionId, job.BaseVersion);

                try
                {
                    await regionService.CreateVersionFromPublishedAsync(job.OwnerUserId, job.RegionId, stoppingToken);
                    _logger.LogInformation("CreateVersionFromPublishedAsync (region) completed: jobId={JobId} regionId={RegionId}", job.Id, job.RegionId);
                }
                catch (Exception versionEx)
                {
                    _logger.LogError(versionEx, "CreateVersionFromPublishedAsync (region) failed: jobId={JobId} regionId={RegionId}", job.Id, job.RegionId);
                    throw;
                }

                job.Status = RegionVersionJobStatus.Completed;
                job.CompletedAtUtc = DateTime.UtcNow;
                job.ErrorMessage = null;
                
                await db.SaveChangesAsync(stoppingToken);
                PublishStatus();

                _logger.LogInformation("RegionVersionJob completed: jobId={JobId} regionId={RegionId} baseVersion={BaseVersion}",
                    job.Id, job.RegionId, job.BaseVersion);

                await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process RegionVersionJob: jobId={JobId} regionId={RegionId}", job.Id, job.RegionId);

                if (job.DequeueCount >= 3)
                {
                    job.Status = RegionVersionJobStatus.Failed;
                    job.ErrorMessage = ex.Message;
                    job.CompletedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                }
                // If DequeueCount < 3, don't delete message - queue will re-deliver it
            }
        } // Scope disposed here - DbContext and all scoped services are cleaned up
    }

    // Note: QueueClient from Azure.Storage.Queues v12+ is stateless and doesn't implement IDisposable.
    // No explicit cleanup needed. Scoped services (DbContext, etc.) are disposed via 'using var scope'.

    private sealed record EpicAggregatesQueuePayload(string JobType, Guid JobId, string? HeroId, string? RegionId, int BaseVersion);
}

