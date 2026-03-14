using System.Text.Json;
using Azure;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryAIGenerateQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<StoryAIGenerateQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public StoryAIGenerateQueueWorker(
        IServiceProvider services,
        ILogger<StoryAIGenerateQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AIGenerate"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-ai-generate-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryAIGenerateQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryAIGenerateQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryAIGenerateQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: TimeSpan.FromSeconds(300),
                    cancellationToken: stoppingToken);

                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                if (message?.MessageId == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    continue;
                }

                StoryAIGenerateQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryAIGenerateQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var job = await db.StoryAIGenerateJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryAIGenerateJobStatus.Completed or StoryAIGenerateJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    job.DequeueCount++;
                    await db.SaveChangesAsync(stoppingToken);

                    job.Status = StoryAIGenerateJobStatus.Running;
                    job.StartedAtUtc = DateTime.UtcNow;
                    job.ProgressMessage = "Generating text...";
                    await db.SaveChangesAsync(stoppingToken);
                    PublishJobEvent(job);

                    if (job.IsPrivateStoryGeneration)
                        await ProcessPrivateStoryJobAsync(scope, db, job, stoppingToken);
                    else
                        await ProcessDraftStoryJobAsync(scope, db, job, stoppingToken);

                    await DeleteMessageSafeAsync(message.MessageId, message.PopReceipt, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("StoryAIGenerateQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryAIGenerateQueueWorker loop. Retrying in 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryAIGenerateQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private async Task ProcessPrivateStoryJobAsync(
        IServiceScope scope,
        XooDbContext db,
        StoryAIGenerateJob job,
        CancellationToken stoppingToken)
    {
        var userProfile = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
        var privateHandler = scope.ServiceProvider.GetRequiredService<IGeneratePrivateStoryHandler>();
        var spentCredits = false;
        var refundReference = $"refund:full-story:{job.Id:N}";

        GeneratePrivateStoryRequest? request = null;
        try
        {
            request = JsonSerializer.Deserialize<GeneratePrivateStoryRequest>(job.RequestJson, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize private story RequestJson for jobId={JobId}", job.Id);
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = "Invalid request payload.";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            return;
        }

        if (request == null || string.IsNullOrWhiteSpace(request.Idea))
        {
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = "Invalid or missing idea.";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            return;
        }

        var spendResult = await userProfile.SpendFullStoryCreditsAsync(
            job.OwnerUserId,
            new SpendCreditsRequest { Amount = 1, Reference = "full-story-generation" });
        if (!spendResult.Success)
        {
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = spendResult.ErrorMessage ?? "Insufficient full story generation credits.";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            return;
        }
        spentCredits = true;

        try
        {
            job.ProgressMessage = request.GenerateImages || request.GenerateAudio
                ? "Generating images and audio..."
                : "Finalizing story...";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);

            var response = await privateHandler.ExecuteAsync(
                request,
                job.OwnerUserId,
                job.OwnerFirstName ?? string.Empty,
                job.OwnerLastName ?? string.Empty,
                job.RequestedByEmail,
                stoppingToken);

            job.StoryId = response.StoryId;
            job.Status = StoryAIGenerateJobStatus.Completed;
            job.CompletedAtUtc = DateTime.UtcNow;
            var warnings = response.Warnings ?? [];
            if (warnings.Count > 0)
            {
                job.ProgressMessage = BuildCompletedWarningsProgress(warnings);
                // Keep completed jobs non-error from UI perspective.
                // Warnings are surfaced through progress message only.
                job.ErrorCode = null;
            }
            else
            {
                job.ProgressMessage = null;
                job.ErrorCode = null;
            }
            job.ErrorMessage = null;
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            _logger.LogInformation("PrivateStoryAIGenerateJob completed: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PrivateStoryAIGenerateJob failed: jobId={JobId}", job.Id);
            if (spentCredits)
            {
                var refund = await userProfile.RefundFullStoryCreditsAsync(
                    job.OwnerUserId,
                    new SpendCreditsRequest { Amount = 1, Reference = refundReference });
                if (!refund.Success)
                {
                    _logger.LogError(
                        "Failed to refund full-story credits for failed jobId={JobId}. reason={Reason}",
                        job.Id,
                        refund.ErrorMessage);
                }
            }
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = ex.Message;
            job.ErrorCode = ResolveErrorCode(ex);
            if (job.ErrorCode == "RateLimitExceeded" && (string.IsNullOrWhiteSpace(job.ErrorMessage) || !job.ErrorMessage.Contains("429")))
                job.ErrorMessage = "Generation stopped: rate limit exceeded (429). Please try again in a few minutes.";
            job.ProgressMessage = null;
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
        }

    }

    private async Task ProcessDraftStoryJobAsync(
        IServiceScope scope,
        XooDbContext db,
        StoryAIGenerateJob job,
        CancellationToken stoppingToken)
    {
        var handler = scope.ServiceProvider.GetRequiredService<IGenerateFullStoryDraftHandler>();

        GenerateFullStoryDraftRequest? request = null;
        try
        {
            request = JsonSerializer.Deserialize<GenerateFullStoryDraftRequest>(job.RequestJson, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize RequestJson for jobId={JobId}", job.Id);
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = "Invalid request payload.";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            return;
        }

        if (request == null || string.IsNullOrWhiteSpace(request.ApiKey))
        {
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = "Invalid or missing request (ApiKey required).";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            return;
        }

        try
        {
            if (request.GenerateImages || request.GenerateAudio)
                job.ProgressMessage = "Generating images and audio...";
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);

            var response = await handler.ExecuteAsync(
                request,
                job.OwnerUserId,
                job.OwnerFirstName ?? string.Empty,
                job.OwnerLastName ?? string.Empty,
                job.RequestedByEmail,
                stoppingToken);

            job.StoryId = response.StoryId;
            job.Status = StoryAIGenerateJobStatus.Completed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ProgressMessage = null;
            job.ErrorMessage = null;
            job.ErrorCode = null;
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
            _logger.LogInformation("StoryAIGenerateJob completed: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryAIGenerateJob failed: jobId={JobId}", job.Id);
            job.Status = StoryAIGenerateJobStatus.Failed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = ex.Message;
            job.ErrorCode = ResolveErrorCode(ex);
            if (job.ErrorCode == "RateLimitExceeded" && (string.IsNullOrWhiteSpace(job.ErrorMessage) || !job.ErrorMessage.Contains("429")))
                job.ErrorMessage = "Generation stopped: rate limit exceeded (429). Please try again in a few minutes.";
            job.ProgressMessage = null;
            await db.SaveChangesAsync(stoppingToken);
            PublishJobEvent(job);
        }

    }

    private async Task DeleteMessageSafeAsync(string messageId, string popReceipt, CancellationToken ct)
    {
        try
        {
            await _queueClient.DeleteMessageAsync(messageId, popReceipt, ct);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogWarning(
                "Queue message already deleted (404). messageId={MessageId} queue={QueueName}",
                messageId,
                _queueClient.Name);
        }
    }

    private void PublishJobEvent(StoryAIGenerateJob job)
    {
        _jobEvents.Publish(JobTypes.StoryAIGenerate, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            status = job.Status,
            progressMessage = job.ProgressMessage,
            queuedAtUtc = job.QueuedAtUtc,
            startedAtUtc = job.StartedAtUtc,
            completedAtUtc = job.CompletedAtUtc,
            errorMessage = job.ErrorMessage,
            errorCode = job.ErrorCode
        });
    }

    private static bool IsRateLimitException(Exception ex)
    {
        var message = (ex.Message + " " + ex.InnerException?.Message).ToLowerInvariant();
        return message.Contains("429") || message.Contains("too many requests") || message.Contains("rate limit");
    }

    private static string? ResolveErrorCode(Exception ex)
    {
        if (ex is ChildSafetyPolicyViolationException)
            return "ChildSafetyPolicyViolation";

        if (ex is GoogleImageGenerationException imageEx)
            return imageEx.ErrorCode;

        if (IsRateLimitException(ex))
            return "RateLimitExceeded";

        if (ex is InvalidOperationException && (ex.Message.Contains("GoogleAI", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("ApiKey", StringComparison.OrdinalIgnoreCase)))
            return "GoogleApiKeyNotConfigured";

        return null;
    }

    private static string BuildCompletedWarningsProgress(List<string> warnings)
    {
        var text = "Completed with warnings: " + string.Join(" | ", warnings);
        return text.Length > 900 ? text[..900] + "..." : text;
    }

    private sealed record StoryAIGenerateQueuePayload(Guid JobId);
}
