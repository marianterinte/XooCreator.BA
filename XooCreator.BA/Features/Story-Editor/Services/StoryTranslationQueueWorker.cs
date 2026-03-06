using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Features.System.Services;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryTranslationQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryTranslationQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public StoryTranslationQueueWorker(
        IServiceProvider services,
        ILogger<StoryTranslationQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["Translation"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-translation-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryTranslationQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryTranslationQueueWorker started. QueueName={QueueName}", _queueClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryTranslationQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: TimeSpan.FromMinutes(15),
                    cancellationToken: stoppingToken);

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

                StoryTranslationQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryTranslationQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize translation queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.StoryId))
                {
                    _logger.LogWarning("Invalid translation payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var translationService = scope.ServiceProvider.GetRequiredService<IStoryTranslationService>();

                    var job = await db.StoryTranslationJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryTranslationJobStatus.Completed or StoryTranslationJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    var maintenanceService = scope.ServiceProvider.GetRequiredService<IStoryCreatorMaintenanceService>();
                    if (await maintenanceService.IsStoryCreatorDisabledAsync(stoppingToken))
                    {
                        _logger.LogInformation("Story Creator is in maintenance; skipping job. jobId={JobId} storyId={StoryId} messageId={MessageId}", job.Id, job.StoryId, message.MessageId);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.StoryTranslation, job.Id, new
                        {
                            jobId = job.Id,
                            storyId = job.StoryId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            fieldsTranslated = job.FieldsTranslated,
                            updatedLanguagesJson = job.UpdatedLanguagesJson
                        });
                    }

                    job.StartedAtUtc = DateTime.UtcNow;
                    job.Status = StoryTranslationJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing StoryTranslationJob: jobId={JobId} storyId={StoryId} refLang={RefLang}",
                            job.Id, job.StoryId, job.ReferenceLanguage);

                        var targetLanguages = JsonSerializer.Deserialize<List<string>>(job.TargetLanguagesJson ?? "[]") ?? new List<string>();
                        List<string>? selectedTileIds = null;
                        if (!string.IsNullOrWhiteSpace(job.SelectedTileIdsJson))
                        {
                            try
                            {
                                selectedTileIds = JsonSerializer.Deserialize<List<string>>(job.SelectedTileIdsJson);
                            }
                            catch
                            {
                                // ignore invalid json, treat as translate all
                            }
                        }

                        if (string.IsNullOrWhiteSpace(job.ApiKeyOverride))
                        {
                            throw new InvalidOperationException("Translation job has no API key.");
                        }

                        var result = await translationService.TranslateStoryAsync(
                            job.StoryId,
                            job.ReferenceLanguage,
                            targetLanguages,
                            job.ApiKeyOverride,
                            selectedTileIds,
                            stoppingToken);

                        job.Status = StoryTranslationJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        job.FieldsTranslated = result.FieldsTranslated;
                        job.UpdatedLanguagesJson = JsonSerializer.Serialize(result.UpdatedLanguages);

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("Successfully completed StoryTranslationJob: jobId={JobId} storyId={StoryId} fieldsTranslated={FieldsTranslated}",
                            job.Id, job.StoryId, result.FieldsTranslated);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process StoryTranslationJob: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);

                        job.Status = StoryTranslationJobStatus.Failed;
                        job.ErrorMessage = ex.Message;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryTranslationQueueWorker. Retrying in 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryTranslationQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private sealed record StoryTranslationQueuePayload(Guid JobId, string StoryId);
}
