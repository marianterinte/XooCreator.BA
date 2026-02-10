using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryImageExportQueueWorker : BackgroundService
{
    private const int MaxConcurrency = 3;

    private readonly IServiceProvider _services;
    private readonly ILogger<StoryImageExportQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public StoryImageExportQueueWorker(
        IServiceProvider services,
        ILogger<StoryImageExportQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["ImageExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-image-export-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryImageExportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryImageExportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryImageExportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: TimeSpan.FromSeconds(120),
                    cancellationToken: stoppingToken);

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

                _logger.LogInformation("Received image export message from queue: messageId={MessageId} queueName={QueueName}",
                    message.MessageId, _queueClient.Name);

                StoryImageExportQueuePayload? payload = null;
                try
                {
                    payload = JsonSerializer.Deserialize<StoryImageExportQueuePayload>(message.MessageText);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deserialize queue message: {MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                if (payload == null || payload.JobId == Guid.Empty || string.IsNullOrWhiteSpace(payload.StoryId))
                {
                    _logger.LogWarning("Invalid payload for messageId={MessageId}", message.MessageId);
                    await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    continue;
                }

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
                    var crafts = scope.ServiceProvider.GetRequiredService<IStoryCraftsRepository>();
                    var imageService = scope.ServiceProvider.GetRequiredService<IGoogleImageService>();
                    var sas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();

                    var job = await db.StoryImageExportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryImageExportJobStatus.Completed or StoryImageExportJobStatus.Failed)
                    {
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                        continue;
                    }

                    void PublishStatus()
                    {
                        _jobEvents.Publish(JobTypes.StoryImageExport, job.Id, new
                        {
                            jobId = job.Id,
                            storyId = job.StoryId,
                            status = job.Status,
                            queuedAtUtc = job.QueuedAtUtc,
                            startedAtUtc = job.StartedAtUtc,
                            completedAtUtc = job.CompletedAtUtc,
                            errorMessage = job.ErrorMessage,
                            zipDownloadUrl = (string?)null,
                            zipFileName = job.ZipFileName,
                            zipSizeBytes = job.ZipSizeBytes,
                            imageCount = job.ImageCount
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryImageExportJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing StoryImageExportJob: jobId={JobId} storyId={StoryId} locale={Locale}",
                            job.Id, job.StoryId, job.Locale);

                        var exportResult = await ProcessImageExportAsync(job, crafts, imageService, stoppingToken);

                        var zipBlobPath = $"exports/images/{job.Id}/{exportResult.FileName}";
                        var blobClient = sas.GetBlobClient(sas.DraftContainer, zipBlobPath);
                        await blobClient.UploadAsync(new BinaryData(exportResult.ZipBytes), overwrite: true, stoppingToken);

                        job.Status = StoryImageExportJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        job.ZipBlobPath = zipBlobPath;
                        job.ZipFileName = exportResult.FileName;
                        job.ZipSizeBytes = exportResult.ZipSizeBytes;
                        job.ImageCount = exportResult.ImageCount;

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("Successfully completed StoryImageExportJob: jobId={JobId} storyId={StoryId} zipSizeBytes={ZipSizeBytes}",
                            job.Id, job.StoryId, exportResult.ZipSizeBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process StoryImageExportJob: jobId={JobId} storyId={StoryId}",
                            job.Id, job.StoryId);

                        var errorMessage = ExtractErrorMessage(ex);

                        job.Status = StoryImageExportJobStatus.Failed;
                        job.ErrorMessage = errorMessage;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryImageExportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryImageExportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryImageExportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryImageExportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private async Task<ImageExportResult> ProcessImageExportAsync(
        StoryImageExportJob job,
        IStoryCraftsRepository crafts,
        IGoogleImageService imageService,
        CancellationToken ct)
    {
        var craft = await crafts.GetWithLanguageAsync(job.StoryId, job.Locale, ct);
        if (craft == null)
        {
            throw new InvalidOperationException($"StoryCraft not found: {job.StoryId}");
        }

        if (string.IsNullOrWhiteSpace(job.ApiKeyOverride))
        {
            throw new InvalidOperationException("API key is required for image export. Please provide your Google API key in the Generate Images modal.");
        }

        HashSet<Guid>? selectedTileIds = null;
        if (!string.IsNullOrWhiteSpace(job.SelectedTileIdsJson))
        {
            try
            {
                var ids = JsonSerializer.Deserialize<List<Guid>>(job.SelectedTileIdsJson);
                if (ids != null && ids.Count > 0)
                {
                    selectedTileIds = new HashSet<Guid>(ids);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse SelectedTileIdsJson for job {JobId}, will process all pages", job.Id);
            }
        }

        var pageOrQuizTypes = new[] { "page", "quiz" };
        var allPages = craft.Tiles
            .Where(t => pageOrQuizTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .Select((tile, index) => new ImagePage(tile, index + 1, ResolveTileText(tile, job.Locale)))
            .ToList();

        var pages = allPages
            .Where(p => (selectedTileIds == null || selectedTileIds.Contains(p.Tile.Id))
                        && !string.IsNullOrWhiteSpace(p.Text))
            .ToList();

        if (pages.Count == 0)
        {
            throw new InvalidOperationException($"No page tiles with text found for story: {job.StoryId}");
        }

        var storyContextJson = BuildStoryContextJson(craft, job.Locale);
        var extraInstructions = job.ExtraInstructions;
        byte[]? referenceImageBytes = null;
        if (!string.IsNullOrWhiteSpace(job.ReferenceImageBase64))
        {
            try
            {
                referenceImageBytes = Convert.FromBase64String(job.ReferenceImageBase64);
            }
            catch
            {
                throw new InvalidOperationException("Reference image data is invalid. Please re-upload the image and try again.");
            }
        }

        if (referenceImageBytes != null && referenceImageBytes.Length > 0 && string.IsNullOrWhiteSpace(extraInstructions))
        {
            extraInstructions = "Use the reference image as a style guide and keep characters consistent. Generate each page as a scene from the story.";
        }

        try
        {
            var testPage = pages[0];
            await imageService.GenerateStoryImageAsync(
                storyContextJson, testPage.Text, job.Locale,
                extraInstructions, referenceImageBytes, job.ReferenceImageMimeType, ct, job.ApiKeyOverride, job.ImageModel);
        }
        catch (Exception ex)
        {
            var errorMessage = ExtractErrorMessage(ex);
            if (errorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("API key", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(errorMessage);
            }
            _logger.LogWarning(ex, "Test request for first page failed, but continuing: {ErrorMessage}", errorMessage);
        }

        var results = new List<ImagePageResult>();
        var errors = new List<(int PageNumber, string ErrorMessage)>();
        using var semaphore = new SemaphoreSlim(MaxConcurrency, MaxConcurrency);

        var tasks = pages.Select(async page =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var (imageData, mimeType) = await imageService.GenerateStoryImageAsync(
                    storyContextJson, page.Text, job.Locale,
                    extraInstructions, referenceImageBytes, job.ReferenceImageMimeType, ct, job.ApiKeyOverride, job.ImageModel);
                var ext = mimeType?.Contains("png", StringComparison.OrdinalIgnoreCase) == true ? "png" : "jpg";
                lock (results)
                {
                    results.Add(new ImagePageResult(page.Index, imageData, ext));
                }
            }
            catch (Exception ex)
            {
                lock (errors)
                {
                    var errMsg = ExtractErrorMessage(ex);
                    errors.Add((page.Index, errMsg));
                    _logger.LogError(ex, "Failed to generate image for page {PageNumber} in story {StoryId}: {ErrorMessage}",
                        page.Index, job.StoryId, errMsg);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(tasks);

        if (results.Count == 0 && errors.Count > 0)
        {
            var quotaErrors = errors.Where(e =>
                e.ErrorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                e.ErrorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase)).ToList();
            if (quotaErrors.Count == errors.Count && quotaErrors.Count > 0)
            {
                throw new InvalidOperationException($"Failed to generate images: {quotaErrors[0].ErrorMessage}");
            }
            var errorSummary = string.Join("; ", errors.Select(e => $"Page {e.PageNumber}: {e.ErrorMessage}"));
            throw new InvalidOperationException($"Failed to generate images for all pages. Errors: {errorSummary}");
        }

        if (results.Count == 0)
        {
            throw new InvalidOperationException($"No images generated for story: {job.StoryId}");
        }

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var sortedResults = results.OrderBy(r => r.Index).ToList();
            foreach (var result in sortedResults)
            {
                var entryName = $"{result.Index}.{result.Extension}";
                var entry = zip.CreateEntry(entryName, CompressionLevel.Fastest);
                await using var entryStream = entry.Open();
                await entryStream.WriteAsync(result.ImageBytes, ct);
            }
        }

        var zipBytes = ms.ToArray();
        return new ImageExportResult
        {
            ZipBytes = zipBytes,
            FileName = $"{job.StoryId}-draft-images-{job.Locale}.zip",
            ImageCount = results.Count,
            ZipSizeBytes = zipBytes.Length
        };
    }

    private static string BuildStoryContextJson(StoryCraft craft, string locale)
    {
        var lang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var translation = craft.Translations.FirstOrDefault(tr => tr.LanguageCode == lang) ?? craft.Translations.FirstOrDefault();
        var title = translation?.Title ?? string.Empty;
        var summary = translation?.Summary ?? string.Empty;

        var pageTiles = craft.Tiles
            .Where(t => "page".Equals(t.Type, StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToList();

        var tilesList = new List<object>();
        foreach (var tile in pageTiles)
        {
            var tileTr = tile.Translations.FirstOrDefault(tr => tr.LanguageCode == lang) ?? tile.Translations.FirstOrDefault();
            var text = tileTr?.Text ?? tileTr?.Caption ?? string.Empty;
            tilesList.Add(new
            {
                tileId = tile.TileId,
                type = tile.Type,
                sortOrder = tile.SortOrder,
                caption = tileTr?.Caption ?? string.Empty,
                text
            });
        }

        var obj = new Dictionary<string, object?>
        {
            ["storyId"] = craft.StoryId,
            ["title"] = title,
            ["summary"] = summary ?? string.Empty,
            ["tiles"] = tilesList
        };

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
    }

    private static string ResolveTileText(StoryCraftTile tile, string locale)
    {
        var lang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var translation = tile.Translations.FirstOrDefault(tr => tr.LanguageCode == lang)
                          ?? tile.Translations.FirstOrDefault();
        var text = translation?.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(text)) return text!;
        var question = translation?.Question?.Trim();
        if (!string.IsNullOrWhiteSpace(question)) return question!;
        var caption = translation?.Caption?.Trim();
        return caption ?? string.Empty;
    }

    private static string ExtractErrorMessage(Exception ex)
    {
        var currentEx = ex;
        while (currentEx != null)
        {
            var message = currentEx.Message;
            if (message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            {
                return message;
            }
            if (currentEx is HttpRequestException httpEx && message.Contains("400"))
            {
                return "Google API error: Invalid API key or request. Please check or try a different API key.";
            }
            if (message.Contains("API Key") || message.Contains("API key"))
            {
                return "Google API key is invalid or missing. Please provide a valid API key in the modal.";
            }
            if (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
                continue;
            }
            return currentEx.Message;
        }
        return "An error occurred while generating images.";
    }

    private sealed record StoryImageExportQueuePayload(Guid JobId, string StoryId);
    private sealed record ImagePage(StoryCraftTile Tile, int Index, string Text);
    private sealed record ImagePageResult(int Index, byte[] ImageBytes, string Extension);

    private sealed record ImageExportResult
    {
        public required byte[] ZipBytes { get; init; }
        public required string FileName { get; init; }
        public required int ImageCount { get; init; }
        public required long ZipSizeBytes { get; init; }
    }
}
