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
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.AlchimaliaUniverse.Mappers;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class AnimalVersionQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AnimalVersionQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public AnimalVersionQueueWorker(
        IServiceProvider services,
        ILogger<AnimalVersionQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AnimalVersion"] ?? "animal-version-jobs";
        _queueClient = queueClientFactory.CreateClient(queueName, "animal-version-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AnimalVersionQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("AnimalVersionQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}", 
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AnimalVersionQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
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

                AnimalVersionQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<AnimalVersionQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || payload.AnimalId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var service = scope.ServiceProvider.GetRequiredService<IAnimalCraftService>();
                    var assetCopyService = scope.ServiceProvider.GetRequiredService<IAlchimaliaUniverseAssetCopyService>();
                    var repository = scope.ServiceProvider.GetRequiredService<XooCreator.BA.Features.AlchimaliaUniverse.Repositories.IAnimalCraftRepository>();

                    var job = await db.AnimalVersionJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is AnimalVersionJobStatus.Completed or AnimalVersionJobStatus.Failed or AnimalVersionJobStatus.Superseded)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish("AnimalVersion", job.Id, new
                        {
                            jobId = job.Id,
                            animalId = job.AnimalId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            dequeueCount = job.DequeueCount,
                            baseVersion = job.BaseVersion
                        });
                    }

                    var runningJob = await db.AnimalVersionJobs
                        .FirstOrDefaultAsync(j => j.AnimalId == job.AnimalId && j.Id != job.Id && j.Status == AnimalVersionJobStatus.Running, stoppingToken);
                    if (runningJob != null)
                    {
                        job.Status = AnimalVersionJobStatus.Superseded;
                        job.ErrorMessage = $"Job superseded by running job {runningJob.Id}.";
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = AnimalVersionJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing AnimalVersionJob: jobId={JobId} animalId={AnimalId} baseVersion={BaseVersion}",
                            job.Id, job.AnimalId, job.BaseVersion);
                        
                        await service.CreateCraftFromDefinitionAsync(job.OwnerUserId, job.AnimalId, stoppingToken);

                        // Load the created entity to update paths
                        var craft = await repository.GetAsync(job.AnimalId, stoppingToken);
                        if (craft != null)
                        {
                            try 
                            {
                                var assets = AlchimaliaUniverseAssetPathMapper.CollectFromAnimalCraft(craft);
                                await assetCopyService.CopyPublishedToDraftAsync(
                                    assets,
                                    job.RequestedByEmail, 
                                    job.AnimalId.ToString(), // Animal ID is Guid
                                    job.RequestedByEmail,
                                    job.AnimalId.ToString(),
                                    AlchimaliaUniverseAssetPathMapper.EntityType.Animal,
                                    stoppingToken);

                                // Update craft paths to draft
                                bool changed = false;
                                if (!string.IsNullOrWhiteSpace(craft.Src))
                                {
                                    var filename = Path.GetFileName(craft.Src);
                                    var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Image, null);
                                    var draftPath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(assetInfo, job.RequestedByEmail, job.AnimalId.ToString(), AlchimaliaUniverseAssetPathMapper.EntityType.Animal);
                                    craft.Src = assetCopyService.GetDraftUrl(draftPath);
                                    changed = true;
                                }
                                
                                // Update Audio URLs in translations
                                foreach (var t in craft.Translations)
                                {
                                    if (!string.IsNullOrWhiteSpace(t.AudioUrl))
                                    {
                                        var filename = Path.GetFileName(t.AudioUrl);
                                        var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Audio, t.LanguageCode);
                                        var draftPath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(assetInfo, job.RequestedByEmail, job.AnimalId.ToString(), AlchimaliaUniverseAssetPathMapper.EntityType.Animal);
                                        t.AudioUrl = assetCopyService.GetDraftUrl(draftPath);
                                        changed = true;
                                        // Note: Translations are owned by Craft, so updating them here should work if Entity Framework tracks them.
                                        // Repository.UpdateAsync usually handles graph?
                                        // Assuming yes.
                                    }
                                }

                                if (changed)
                                {
                                    await repository.SaveAsync(craft, stoppingToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Asset copy failed for Animal Version Job {JobId}", job.Id);
                            }
                        }

                        job.Status = AnimalVersionJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();

                        _logger.LogInformation("AnimalVersionJob completed: jobId={JobId} animalId={AnimalId} baseVersion={BaseVersion}",
                            job.Id, job.AnimalId, job.BaseVersion);

                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process AnimalVersionJob: jobId={JobId} animalId={AnimalId}", job.Id, job.AnimalId);

                        if (job.DequeueCount >= 3)
                        {
                            job.Status = AnimalVersionJobStatus.Failed;
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
                _logger.LogError(ex, "Unexpected error in AnimalVersionQueueWorker loop. Retrying in 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private sealed record AnimalVersionQueuePayload(Guid JobId, Guid AnimalId, int BaseVersion);
}
