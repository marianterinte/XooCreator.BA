using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.AlchimaliaUniverse.Mappers;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.AlchimaliaUniverse.Mappers;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class HeroDefinitionVersionQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<HeroDefinitionVersionQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public HeroDefinitionVersionQueueWorker(
        IServiceProvider services,
        ILogger<HeroDefinitionVersionQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        // Use the same queue name as in HeroDefinitionVersionQueue
        var queueName = configuration.GetSection("AzureStorage:Queues")?["HeroVersion"] ?? "hero-version-jobs";
        _queueClient = queueClientFactory.CreateClient(queueName, "hero-version-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HeroDefinitionVersionQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("HeroDefinitionVersionQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}", 
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HeroDefinitionVersionQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
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
                    _logger.LogWarning("Received message with null MessageId. QueueName={QueueName}", _queueClient.Name);
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                _logger.LogInformation("Received message from queue: messageId={MessageId} queueName={QueueName}", 
                    message.MessageId, _queueClient.Name);

                HeroDefinitionVersionQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<HeroDefinitionVersionQueuePayload>(message.MessageText);
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
                    var assetCopyService = scope.ServiceProvider.GetRequiredService<IAlchimaliaUniverseAssetCopyService>();
                    var repository = scope.ServiceProvider.GetRequiredService<XooCreator.BA.Features.AlchimaliaUniverse.Repositories.IHeroDefinitionCraftRepository>();

                    var job = await db.HeroDefinitionVersionJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is HeroDefinitionVersionJobStatus.Completed or HeroDefinitionVersionJobStatus.Failed or HeroDefinitionVersionJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.HeroDefinitionVersion, job.Id, new
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
                    var runningJob = await db.HeroDefinitionVersionJobs
                        .FirstOrDefaultAsync(j => j.HeroId == job.HeroId && j.Id != job.Id && j.Status == HeroDefinitionVersionJobStatus.Running, stoppingToken);
                    if (runningJob != null)
                    {
                        job.Status = HeroDefinitionVersionJobStatus.Superseded;
                        job.ErrorMessage = $"Job superseded by running job {runningJob.Id}.";
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = HeroDefinitionVersionJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing HeroDefinitionVersionJob: jobId={JobId} heroId={HeroId} baseVersion={BaseVersion}",
                            job.Id, job.HeroId, job.BaseVersion);

                        // Create craft (points to published assets initially)
                        var owner = await db.AlchimaliaUsers
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == job.OwnerUserId, stoppingToken);
                        var isAdmin = owner?.Roles?.Contains(UserRole.Admin) == true || owner?.Role == UserRole.Admin;
                        var craftDto = await service.CreateCraftFromDefinitionAsync(job.OwnerUserId, job.HeroId, allowAdminOverride: isAdmin, stoppingToken);

                        // Load the created entity to update paths
                        var craft = await repository.GetAsync(job.HeroId, stoppingToken);
                        if (craft != null)
                        {
                            try 
                            {
                                var assets = AlchimaliaUniverseAssetPathMapper.CollectFromHeroCraft(craft);
                                await assetCopyService.CopyPublishedToDraftAsync(
                                    assets,
                                    job.RequestedByEmail, // Using requester email as proxy for published path owner if mapped that way
                                    job.HeroId,
                                    job.RequestedByEmail,
                                    job.HeroId,
                                    AlchimaliaUniverseAssetPathMapper.EntityType.Hero,
                                    stoppingToken);

                                // Update craft paths to draft
                                if (!string.IsNullOrWhiteSpace(craft.Image))
                                {
                                    var filename = Path.GetFileName(craft.Image);
                                    var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Image, null);
                                    var draftPath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(assetInfo, job.RequestedByEmail, job.HeroId, AlchimaliaUniverseAssetPathMapper.EntityType.Hero);
                                    var draftUrl = assetCopyService.GetDraftUrl(draftPath);
                                    craft.Image = draftUrl;
                                    await repository.SaveAsync(craft, stoppingToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Asset copy failed for Hero Version Job {JobId}", job.Id);
                            }
                        }

                        job.Status = HeroDefinitionVersionJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();

                        _logger.LogInformation("HeroDefinitionVersionJob completed: jobId={JobId} heroId={HeroId} baseVersion={BaseVersion}",
                            job.Id, job.HeroId, job.BaseVersion);

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process HeroDefinitionVersionJob: jobId={JobId} heroId={HeroId}", job.Id, job.HeroId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = HeroDefinitionVersionJobStatus.Failed;
                            job.ErrorMessage = ex.Message;
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            PublishStatus();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in HeroDefinitionVersionQueueWorker loop. Retrying in 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private sealed record HeroDefinitionVersionQueuePayload(Guid JobId, string HeroId, int BaseVersion);
}
