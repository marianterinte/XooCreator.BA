using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using XooCreator.BA.Features.System.Services;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryAudioExportQueueWorker : BackgroundService
{
    private const int MaxChunkLength = 5000;
    private const int MaxConcurrency = 30;
    private const int SampleRate = 24000;
    private const short BitsPerSample = 16;
    private const short Channels = 1;
    private const double DefaultSilenceSeconds = 0.5;

    private readonly IServiceProvider _services;
    private readonly ILogger<StoryAudioExportQueueWorker> _logger;
    private readonly QueueClient _queueClient;
    private readonly IJobEventsHub _jobEvents;

    public StoryAudioExportQueueWorker(
        IServiceProvider services,
        ILogger<StoryAudioExportQueueWorker> logger,
        IConfiguration configuration,
        IJobEventsHub jobEvents,
        IAzureQueueClientFactory queueClientFactory)
    {
        _services = services;
        _logger = logger;
        _jobEvents = jobEvents;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["AudioExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "story-audio-export-queue");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoryAudioExportQueueWorker initializing... QueueName={QueueName}", _queueClient.Name);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("StoryAudioExportQueueWorker started. QueueName={QueueName} QueueUri={QueueUri}",
                _queueClient.Name, _queueClient.Uri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StoryAudioExportQueueWorker failed to create/connect to queue. QueueName={QueueName}. Retrying in 30 seconds.", _queueClient.Name);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1,
                    visibilityTimeout: TimeSpan.FromSeconds(60),
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

                _logger.LogInformation("Received audio export message from queue: messageId={MessageId} queueName={QueueName}",
                    message.MessageId, _queueClient.Name);

                StoryAudioExportQueuePayload? payload = null;
                try
                {
                    payload =JsonSerializer.Deserialize<StoryAudioExportQueuePayload>(message.MessageText);
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
                    var audioService = scope.ServiceProvider.GetRequiredService<IGoogleAudioGeneratorService>();
                    var sas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();

                    var job = await db.StoryAudioExportJobs.FirstOrDefaultAsync(j => j.Id == payload.JobId, stoppingToken);
                    if (job == null || job.Status is StoryAudioExportJobStatus.Completed or StoryAudioExportJobStatus.Failed)
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
                        _jobEvents.Publish(JobTypes.StoryAudioExport, job.Id, new
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
                            audioCount = job.AudioCount
                        });
                    }

                    job.DequeueCount += 1;
                    job.StartedAtUtc ??= DateTime.UtcNow;
                    job.Status = StoryAudioExportJobStatus.Running;
                    await db.SaveChangesAsync(stoppingToken);
                    PublishStatus();

                    try
                    {
                        _logger.LogInformation("Processing StoryAudioExportJob: jobId={JobId} storyId={StoryId} locale={Locale}",
                            job.Id, job.StoryId, job.Locale);

                        var exportResult = await ProcessAudioExportAsync(job, crafts, audioService, stoppingToken);

                        // Upload ZIP to blob storage (memory is automatically freed after this method completes)
                        var zipBlobPath = $"exports/audio/{job.Id}/{exportResult.FileName}";
                        var blobClient = sas.GetBlobClient(sas.DraftContainer, zipBlobPath);
                        await blobClient.UploadAsync(new BinaryData(exportResult.ZipBytes), overwrite: true, stoppingToken);
                        
                        // Note: exportResult.ZipBytes memory is freed when ProcessAudioExportAsync completes
                        // The ZIP file remains in blob storage until manually deleted or cleaned up by retention policy

                        job.Status = StoryAudioExportJobStatus.Completed;
                        job.CompletedAtUtc = DateTime.UtcNow;
                        job.ErrorMessage = null;
                        job.ZipBlobPath = zipBlobPath;
                        job.ZipFileName = exportResult.FileName;
                        job.ZipSizeBytes = exportResult.ZipSizeBytes;
                        job.AudioCount = exportResult.AudioCount;

                        await db.SaveChangesAsync(stoppingToken);
                        PublishStatus();
                        await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);

                        _logger.LogInformation("Successfully completed StoryAudioExportJob: jobId={JobId} storyId={StoryId} zipSizeBytes={ZipSizeBytes}",
                            job.Id, job.StoryId, exportResult.ZipSizeBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process StoryAudioExportJob: jobId={JobId} storyId={StoryId}",
                            job.Id, job.StoryId);

                        // Extract user-friendly error message
                        var errorMessage = ExtractErrorMessage(ex);
                        
                        // Mark job as failed immediately (don't retry for API errors)
                        job.Status = StoryAudioExportJobStatus.Failed;
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
                _logger.LogInformation("StoryAudioExportQueueWorker stopping due to cancellation request.");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryAudioExportQueueWorker stopping due to task cancellation.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in StoryAudioExportQueueWorker loop. ExceptionType={ExceptionType}. Retrying in 10 seconds.",
                    ex.GetType().FullName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("StoryAudioExportQueueWorker has stopped. QueueName={QueueName}", _queueClient.Name);
    }

    private async Task<AudioExportResult> ProcessAudioExportAsync(
        StoryAudioExportJob job,
        IStoryCraftsRepository crafts,
        IGoogleAudioGeneratorService audioService,
        CancellationToken ct)
    {
        var craft = await crafts.GetWithLanguageAsync(job.StoryId, job.Locale, ct);
        if (craft == null)
        {
            throw new InvalidOperationException($"StoryCraft not found: {job.StoryId}");
        }

        // Parse selected tile IDs if provided
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

        // Build audio items: one per page/quiz tile, one per dialog node (replica text) so full export includes dialogues
        var pageOrQuizOrDialogTypes = new[] { "page", "quiz", "dialog" };
        var locale = (job.Locale ?? string.Empty).Trim().ToLowerInvariant();
        var allPages = new List<AudioPage>();
        var runningIndex = 1;
        foreach (var tile in craft.Tiles
            .Where(t => pageOrQuizOrDialogTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder))
        {
            if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && tile.DialogTile != null)
            {
                foreach (var node in tile.DialogTile.Nodes.OrderBy(n => n.SortOrder))
                {
                    var nodeText = node.Translations
                        .FirstOrDefault(nt => string.Equals(nt.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                        ?.Text?.Trim() ?? node.Translations.FirstOrDefault()?.Text?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(nodeText))
                    {
                        allPages.Add(new AudioPage(tile, runningIndex++, nodeText, node.NodeId, node.SpeakerType));
                    }
                }
            }
            else
            {
                var tileText = ResolveTileText(tile, job.Locale);
                if (!string.IsNullOrWhiteSpace(tileText))
                {
                    allPages.Add(new AudioPage(tile, runningIndex++, tileText));
                }
            }
        }

        // Filter by selected tile IDs if provided (dialog tiles: include all their nodes if tile is selected)
        var pages = allPages
            .Where(p => selectedTileIds == null || selectedTileIds.Contains(p.Tile.Id))
            .ToList();

        if (pages.Count == 0)
        {
            throw new InvalidOperationException($"No page tiles with text found for story: {job.StoryId}");
        }

        // Test first page to detect quota errors early
        // This helps users know immediately if they need to use a different API key
        try
        {
            var testPage = pages[0];
            await GenerateAudioBytesAsync(testPage.Text, job.Locale, audioService, job.ApiKeyOverride, job.TtsModel, ct);
        }
        catch (Exception ex)
        {
            var errorMessage = ExtractErrorMessage(ex);
            // If it's a quota error, fail immediately with clear message
            if (errorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("different API key", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(errorMessage);
            }
            // For other errors, log but continue (might be transient)
            _logger.LogWarning(ex, "Test request for first page failed, but continuing: {ErrorMessage}", errorMessage);
        }

        var results = new List<AudioPageResult>();
        var errors = new List<(int PageNumber, string ErrorMessage)>();
        using var semaphore = new SemaphoreSlim(MaxConcurrency, MaxConcurrency);

        var tasks = pages.Select(async page =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var audioBytes = await GenerateAudioBytesAsync(page.Text, job.Locale, audioService, job.ApiKeyOverride, job.TtsModel, ct);
                var fileName = BuildExportFileName(page);
                lock (results)
                {
                    results.Add(new AudioPageResult(page.Index, audioBytes, "wav", fileName));
                }
            }
            catch (Exception ex)
            {
                lock (errors)
                {
                    var errorMessage = ExtractErrorMessage(ex);
                    errors.Add((page.Index, errorMessage));
                    _logger.LogError(ex, "Failed to generate audio for page {PageNumber} in story {StoryId}: {ErrorMessage}",
                        page.Index, job.StoryId, errorMessage);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(tasks);

        // Check if quota errors occurred
        var quotaErrors = errors.Where(e => e.ErrorMessage.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                                           e.ErrorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                                           e.ErrorMessage.Contains("different API key", StringComparison.OrdinalIgnoreCase)).ToList();

        // If all pages failed, throw an error with details
        if (results.Count == 0 && errors.Count > 0)
        {
            // If all failures are quota-related, provide a clear message
            if (quotaErrors.Count == errors.Count && quotaErrors.Count > 0)
            {
                var quotaMessage = quotaErrors[0].ErrorMessage; // Use first quota error message
                throw new InvalidOperationException($"Failed to generate audio: {quotaMessage}");
            }
            
            var errorSummary = string.Join("; ", errors.Select(e => $"Page {e.PageNumber}: {e.ErrorMessage}"));
            throw new InvalidOperationException($"Failed to generate audio for all pages. Errors: {errorSummary}");
        }

        // If some pages failed, log warning but continue
        if (errors.Count > 0)
        {
            // If quota errors occurred, include a warning message
            if (quotaErrors.Count > 0)
            {
                var quotaMessage = quotaErrors[0].ErrorMessage;
                _logger.LogWarning("Some pages failed due to quota limits: {QuotaErrorCount} out of {ErrorCount} errors. " +
                    "JobId={JobId}. Message: {QuotaMessage}",
                    quotaErrors.Count, errors.Count, job.Id, quotaMessage);
            }
            else
            {
                _logger.LogWarning("Some pages failed to generate audio: {ErrorCount} out of {TotalPages}. JobId={JobId}",
                    errors.Count, pages.Count, job.Id);
            }
        }

        // If no audio files were generated at all, throw error
        if (results.Count == 0)
        {
            throw new InvalidOperationException($"No audio files generated for story: {job.StoryId}");
        }

        // Create ZIP in memory (automatically freed when method completes)
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Sort by slot index; filename is descriptive for dialog (e.g. 04_n1_hero.wav), numeric for page/quiz (01.wav)
            var sortedResults = results.OrderBy(r => r.Index).ToList();
            foreach (var result in sortedResults)
            {
                var entry = zip.CreateEntry(result.FileName, CompressionLevel.Fastest);
                await using var entryStream = entry.Open();
                await entryStream.WriteAsync(result.AudioBytes, ct);
            }
        }

        var zipBytes = ms.ToArray();
        // Memory stream is disposed here, freeing memory
        return new AudioExportResult
        {
            ZipBytes = zipBytes,
            FileName = $"{job.StoryId}-draft-audio-{job.Locale}.zip",
            AudioCount = results.Count,
            ZipSizeBytes = zipBytes.Length
        };
    }

    /// <summary>Standardized ZIP name: {slot:D2}_{type}_{rest}.wav — e.g. 01_page.wav, 04_dialog_n1_hero.wav. Import uses leading digits only.</summary>
    private static string BuildExportFileName(AudioPage page)
    {
        var slot = page.Index.ToString("D2");
        var type = GetTileTypeSlug(page.Tile.Type);
        if (!string.IsNullOrEmpty(page.NodeId) && !string.IsNullOrEmpty(page.SpeakerType))
        {
            var nodePart = SanitizeFileNamePart(page.NodeId);
            var speakerPart = SanitizeFileNamePart(page.SpeakerType);
            return $"{slot}_dialog_{nodePart}_{speakerPart}.wav";
        }
        return $"{slot}_{type}.wav";
    }

    private static string GetTileTypeSlug(string? tileType)
    {
        if (string.IsNullOrWhiteSpace(tileType)) return "page";
        return tileType.Trim().ToLowerInvariant() switch
        {
            "quiz" => "quiz",
            "dialog" => "dialog",
            _ => "page"
        };
    }

    private static string SanitizeFileNamePart(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return Regex.Replace(s.Trim().ToLowerInvariant(), @"[^a-z0-9\-_]", "_");
    }

    private static string ResolveTileText(StoryCraftTile tile, string locale)
    {
        var lang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var translation = tile.Translations.FirstOrDefault(tr => tr.LanguageCode == lang)
                          ?? tile.Translations.FirstOrDefault();
        return ResolveDisplayText(translation?.Text, translation?.Question, translation?.Caption);
    }

    private static string ResolveDisplayText(string? text, string? question, string? caption)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text.Trim();
        }

        if (!string.IsNullOrWhiteSpace(question))
        {
            return question.Trim();
        }

        if (!string.IsNullOrWhiteSpace(caption))
        {
            return caption.Trim();
        }

        return string.Empty;
    }

    private static async Task<byte[]> GenerateAudioBytesAsync(
        string text,
        string locale,
        IGoogleAudioGeneratorService audioService,
        string? apiKeyOverride,
        string? ttsModelOverride,
        CancellationToken ct)
    {
        // Paginile goale sunt deja filtrate în ProcessAudioExportAsync, dar verificăm din nou pentru siguranță
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Cannot generate audio for empty text");
        }

        var chunks = SplitTextIntoChunks(text, MaxChunkLength);
        if (chunks.Count == 1)
        {
            // Folosește style instructions null pentru a folosi default-ul din configurație
            var (audioData, _) = await audioService.GenerateAudioAsync(chunks[0], locale, null, null, apiKeyOverride, ttsModelOverride, ct);
            return audioData;
        }

        var pcmData = new List<byte>();
        foreach (var chunk in chunks)
        {
            // Pentru primul chunk, include style instructions; pentru restul, nu (pentru a evita repetarea)
            // Folosim null pentru toate chunk-urile pentru a folosi default-ul din configurație
            string? styleInstructions = null;
            var (audioData, _) = await audioService.GenerateAudioAsync(chunk, locale, null, styleInstructions, apiKeyOverride, ttsModelOverride, ct);
            var chunkPcm = ExtractPcmData(audioData);
            pcmData.AddRange(chunkPcm);
        }

        return WrapPcmAsWav(pcmData.ToArray(), SampleRate, BitsPerSample, Channels);
    }

    private static List<string> SplitTextIntoChunks(string text, int maxLength)
    {
        var chunks = new List<string>();
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        if (sentences.Count == 0)
        {
            return new List<string> { text };
        }

        var builder = new StringBuilder();
        foreach (var sentence in sentences)
        {
            if (builder.Length + sentence.Length + 1 > maxLength)
            {
                if (builder.Length > 0)
                {
                    chunks.Add(builder.ToString().Trim());
                    builder.Clear();
                }

                if (sentence.Length > maxLength)
                {
                    var start = 0;
                    while (start < sentence.Length)
                    {
                        var len = Math.Min(maxLength, sentence.Length - start);
                        chunks.Add(sentence.Substring(start, len).Trim());
                        start += len;
                    }
                }
                else
                {
                    builder.Append(sentence);
                }
            }
            else
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }
                builder.Append(sentence);
            }
        }

        if (builder.Length > 0)
        {
            chunks.Add(builder.ToString().Trim());
        }

        return chunks;
    }

    private static byte[] ExtractPcmData(byte[] wavBytes)
    {
        if (wavBytes.Length < 44)
        {
            throw new InvalidOperationException("Invalid WAV data");
        }

        var index = 12;
        while (index + 8 <= wavBytes.Length)
        {
            var chunkId = Encoding.ASCII.GetString(wavBytes, index, 4);
            var chunkSize = BitConverter.ToInt32(wavBytes, index + 4);
            index += 8;

            if (chunkId == "data")
            {
                if (index + chunkSize > wavBytes.Length)
                {
                    throw new InvalidOperationException("Invalid WAV data chunk");
                }

                var data = new byte[chunkSize];
                Buffer.BlockCopy(wavBytes, index, data, 0, chunkSize);
                return data;
            }

            index += chunkSize;
        }

        throw new InvalidOperationException("WAV data chunk not found");
    }

    private static byte[] CreateSilenceWav(double durationSeconds)
    {
        var totalSamples = (int)(SampleRate * durationSeconds);
        var pcmData = new byte[totalSamples * (BitsPerSample / 8) * Channels];
        return WrapPcmAsWav(pcmData, SampleRate, BitsPerSample, Channels);
    }

    private static byte[] WrapPcmAsWav(byte[] pcmData, int sampleRate, short bitsPerSample, short channels)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        var subChunk2Size = pcmData.Length;
        var chunkSize = 36 + subChunk2Size;
        short audioFormat = 1; // PCM
        var byteRate = sampleRate * channels * (bitsPerSample / 8);
        short blockAlign = (short)(channels * (bitsPerSample / 8));

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(chunkSize);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write(audioFormat);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write(bitsPerSample);

        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(subChunk2Size);
        writer.Write(pcmData);

        writer.Flush();
        return ms.ToArray();
    }

    private static string ExtractErrorMessage(Exception ex)
    {
        // Extract user-friendly error message from exception chain
        var currentEx = ex;
        while (currentEx != null)
        {
            // Check for quota/rate limit errors first (most important)
            var message = currentEx.Message;
            if (message.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("resource exhausted", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("RESOURCE_EXHAUSTED", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("RATE_LIMIT_EXCEEDED", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("QUOTA_EXCEEDED", StringComparison.OrdinalIgnoreCase))
            {
                // If message already contains helpful text, return it as-is
                if (message.Contains("different API key", StringComparison.OrdinalIgnoreCase))
                {
                    return message;
                }
                return "Google API quota limit has been reached. Please try again later or use a different API key in the modal.";
            }

            // Check for HTTP status codes
            if (currentEx is HttpRequestException httpEx)
            {
                if (httpEx.Message.Contains("429") || httpEx.Message.Contains("TooManyRequests"))
                {
                    return "Google API quota limit has been reached. Please try again later or use a different API key in the modal.";
                }
                if (httpEx.Message.Contains("503") || httpEx.Message.Contains("ServiceUnavailable"))
                {
                    return "Google API service is temporarily unavailable (quota may be exhausted). Please try again later or use a different API key in the modal.";
                }
                if (httpEx.Message.Contains("400"))
                {
                    return "Google API error: Invalid API key or request. Please check configuration or try a different API key.";
                }
            }
            
            if (message.Contains("API Key") || message.Contains("API_KEY") || message.Contains("API key"))
            {
                return "Google API key is invalid or missing. Please check configuration or try a different API key in the modal.";
            }

            if (message.Contains("INVALID_ARGUMENT") || message.Contains("Bad Request"))
            {
                return "Invalid request to Google API. Please check API configuration or try a different API key.";
            }

            // Use inner exception if available
            if (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
                continue;
            }

            // Return the most specific error message
            return currentEx.Message;
        }

        return "An error occurred while generating audio.";
    }

    private sealed record StoryAudioExportQueuePayload(Guid JobId, string StoryId);
    private sealed record AudioPage(StoryCraftTile Tile, int Index, string Text, string? NodeId = null, string? SpeakerType = null);
    private sealed record AudioPageResult(int Index, byte[] AudioBytes, string Format, string FileName);

    private sealed record AudioExportResult
    {
        public required byte[] ZipBytes { get; init; }
        public required string FileName { get; init; }
        public required int AudioCount { get; init; }
        public required long ZipSizeBytes { get; init; }
    }
}
