using System.Linq;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XooCreator.BA.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.CreatureBuilder.Services;

public class GenerativeLoiQueueWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<GenerativeLoiQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public GenerativeLoiQueueWorker(
        IServiceProvider services,
        ILogger<GenerativeLoiQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;
        var queueName = configuration.GetSection("AzureStorage:Queues")?["GenerativeLoi"];
        _queueClient = queueClientFactory.CreateClient(queueName, "generative-loi-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("GenerativeLoiQueueWorker starting... QueueName={QueueName}", _queueClient.Name);
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerativeLoiQueueWorker failed to create queue. Retrying in 30s.");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: TimeSpan.FromSeconds(180), cancellationToken: stoppingToken);
                if (messages?.Value == null || messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                var message = messages.Value[0];
                Guid jobId;
                try
                {
                    var payload = JsonSerializer.Deserialize<JsonElement>(message.MessageText);
                    jobId = payload.GetProperty("JobId").GetGuid();
                }
                catch
                {
                    _logger.LogWarning("Invalid GenerativeLoi message, deleting.");
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var job = await db.GenerativeLoiJobs.FirstOrDefaultAsync(j => j.Id == jobId, stoppingToken);
                    if (job == null || job.Status == "Completed" || job.Status == "Failed")
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void Publish()
                    {
                        _jobEvents.Publish(JobTypes.GenerativeLoi, job.Id, new
                        {
                            jobId = job.Id,
                            status = job.Status,
                            progressMessage = job.ProgressMessage,
                            errorMessage = job.ErrorMessage,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            bestiaryItemId = job.BestiaryItemId,
                            resultName = job.ResultName,
                            resultImageUrl = job.ResultImageUrl,
                            resultStoryText = job.ResultStoryText
                        });
                    }

                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = "Running";
                    job.ProgressMessage = "Starting...";
                    await db.SaveChangesAsync(stoppingToken);
                    Publish();

                    try
                    {
                        var wallet = await db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == job.UserId, stoppingToken);
                        if (wallet == null || wallet.GenerativeBalance < 2)
                        {
                            job.Status = "Failed";
                            job.ErrorMessage = "Insufficient generative credits (need 2).";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            Publish();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        var combination = JsonSerializer.Deserialize<Dictionary<string, string>>(job.CombinationJson);
                        if (combination == null || combination.Count == 0)
                        {
                            job.Status = "Failed";
                            job.ErrorMessage = "Invalid combination.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            Publish();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }
                        var lang = string.IsNullOrWhiteSpace(job.Locale) ? "ro-RO" : job.Locale;

                        var userProfile = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
                        var loiImageService = scope.ServiceProvider.GetRequiredService<ILOIImageGenerationService>();
                        var textService = scope.ServiceProvider.GetRequiredService<IGoogleTextService>();
                        var blobSas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();

                        job.ProgressMessage = "Generating image...";
                        await db.SaveChangesAsync(stoppingToken);
                        Publish();

                        var spend1 = await userProfile.SpendGenerativeCreditsAsync(job.UserId, new SpendCreditsRequest { Amount = 1, Reference = "loi-generative-image" });
                        if (!spend1.Success)
                        {
                            job.Status = "Failed";
                            job.ErrorMessage = spend1.ErrorMessage ?? "Insufficient credits for image.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            Publish();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        var (imageBytes, _) = await loiImageService.GenerateCreatureImageAsync(combination, lang, stoppingToken);
                        var blobPath = $"loi-generative/{job.UserId:N}/{job.Id}.png";
                        var container = blobSas.DraftContainer;
                        var blobClient = blobSas.GetBlobClient(container, blobPath);
                        await blobClient.UploadAsync(new BinaryData(imageBytes), overwrite: true, stoppingToken);

                        job.ProgressMessage = "Writing story...";
                        await db.SaveChangesAsync(stoppingToken);
                        Publish();

                        var spend2 = await userProfile.SpendGenerativeCreditsAsync(job.UserId, new SpendCreditsRequest { Amount = 1, Reference = "loi-generative-text" });
                        if (!spend2.Success)
                        {
                            job.Status = "Failed";
                            job.ErrorMessage = spend2.ErrorMessage ?? "Insufficient credits for text.";
                            job.CompletedAtUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync(stoppingToken);
                            Publish();
                            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            continue;
                        }

                        var creatureDesc = BuildCreatureDescription(combination);
                        var systemInstruction = "You write short, kid-friendly texts for a children's app. Safe only: no scary, violent, or mature content. " +
                            "Output format: first line is a creative short name for the creature (one or two words, can be a portmanteau), then a blank line, then 3-5 sentences describing the creature in a magical, gentle way. No titles or labels.";
                        var userContent = $"Hybrid creature: {creatureDesc}. Write a name and a very short story (in the same language as the user, family-friendly).";
                        var storyText = await textService.GenerateContentAsync(systemInstruction, userContent, responseMimeType: "text/plain", ct: stoppingToken);
                        var (name, story) = ParseNameAndStory(storyText, combination);

                        var bestiaryItemId = Guid.NewGuid();
                        combination.TryGetValue("head", out var headVal);
                        combination.TryGetValue("body", out var bodyVal);
                        combination.TryGetValue("arms", out var armsVal);
                        var bestiaryItem = new BestiaryItem
                        {
                            Id = bestiaryItemId,
                            HeadKey = headVal?.Trim() ?? "",
                            BodyKey = bodyVal?.Trim() ?? "",
                            ArmsKey = armsVal?.Trim() ?? "",
                            Name = name,
                            Story = story,
                            ImageBlobPath = blobPath,
                            PartsJson = job.CombinationJson,
                            CreatedAt = DateTime.UtcNow
                        };
                        db.BestiaryItems.Add(bestiaryItem);

                        var translation = new BestiaryItemTranslation
                        {
                            Id = Guid.NewGuid(),
                            BestiaryItemId = bestiaryItemId,
                            LanguageCode = lang.Length >= 2 ? lang[..2].ToLowerInvariant() : "ro",
                            Name = name,
                            Story = story
                        };
                        db.BestiaryItemTranslations.Add(translation);

                        var userBestiary = new UserBestiary
                        {
                            Id = Guid.NewGuid(),
                            UserId = job.UserId,
                            BestiaryItemId = bestiaryItemId,
                            BestiaryType = "generative",
                            DiscoveredAt = DateTime.UtcNow
                        };
                        db.UserBestiary.Add(userBestiary);

                        var resultImageUrl = (await blobSas.GetReadSasAsync(container, blobPath, TimeSpan.FromHours(24), stoppingToken)).ToString();

                        job.Status = "Completed";
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ProgressMessage = null;
                        job.ErrorMessage = null;
                        job.BestiaryItemId = bestiaryItemId;
                        job.ResultName = name;
                        job.ResultImageUrl = resultImageUrl;
                        job.ResultStoryText = story;
                        await db.SaveChangesAsync(stoppingToken);
                        Publish();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("GenerativeLoi job completed: jobId={JobId} bestiaryItemId={BestiaryItemId}", job.Id, bestiaryItemId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "GenerativeLoi job failed: jobId={JobId}", job.Id);
                        job.Status = "Failed";
                        job.ErrorMessage = ex.Message.Length > 1998 ? ex.Message[..1998] : ex.Message;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        Publish();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GenerativeLoiQueueWorker loop error. Retrying in 10s.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        _logger.LogInformation("GenerativeLoiQueueWorker stopped.");
    }

    private static string BuildCreatureDescription(Dictionary<string, string> combination)
    {
        var parts = new List<string>();
        foreach (var kv in combination.OrderBy(x => x.Key))
        {
            var v = (kv.Value ?? "").Trim();
            if (string.IsNullOrEmpty(v) || v == "—") continue;
            var partName = kv.Key.ToLowerInvariant() switch
            {
                "arms" => "limbs",
                _ => kv.Key.ToLowerInvariant()
            };
            parts.Add($"{partName} of {v}");
        }
        return parts.Count > 0 ? string.Join(", ", parts) : "mixed animal features";
    }

    private static (string name, string story) ParseNameAndStory(string raw, Dictionary<string, string> combination)
    {
        var lines = raw?.Split('\n').Select(s => s.Trim()).Where(s => s.Length > 0).ToList() ?? new List<string>();
        var fallbackName = DeriveName(combination);
        if (lines.Count >= 2)
        {
            var name = lines[0];
            var story = string.Join("\n", lines.Skip(1));
            return (name.Length > 128 ? name[..128] : name, story.Length > 10000 ? story[..10000] : story);
        }
        if (lines.Count == 1)
        {
            var s = lines[0];
            if (s.Length > 200) return (fallbackName, s.Length > 10000 ? s[..10000] : s);
            return (s.Length > 128 ? s[..128] : s, string.Empty);
        }
        return (fallbackName, raw?.Length > 10000 ? raw[..10000] ?? string.Empty : (raw ?? string.Empty));
    }

    private static string DeriveName(Dictionary<string, string> combination)
    {
        var labels = combination
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value) && kv.Value.Trim() != "—")
            .Select(kv => kv.Value!.Trim())
            .Distinct()
            .Take(3)
            .ToList();
        if (labels.Count == 0) return "Creature";
        return string.Join("-", labels);
    }
}
