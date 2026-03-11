using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Generates images and/or audio for a full story draft and uploads them to draft blob storage.
/// Mutates the provided EditableStoryDto (tile ImageUrl, AudioUrl, CoverImageUrl).
/// </summary>
public interface IGenerateFullStoryDraftAssetsGenerator
{
    /// <param name="usePublishedPaths">When true, upload to published container and set full paths on dto (for private story).</param>
    Task FillImagesAndAudioAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        bool isOpenAi,
        bool usePublishedPaths = false,
        CancellationToken ct = default);
}

public sealed class GenerateFullStoryDraftAssetsGenerator : IGenerateFullStoryDraftAssetsGenerator
{
    private readonly IBlobSasService _blobSas;
    private readonly IGoogleImageService _googleImage;
    private readonly IOpenAIImageWithApiKey _openAIImage;
    private readonly ISequentialStoryImageGenerator _sequentialImageGenerator;
    private readonly IGoogleAudioGeneratorService _googleAudio;
    private readonly IOpenAIAudioWithApiKey _openAIAudio;
    private readonly ILogger<GenerateFullStoryDraftAssetsGenerator> _logger;

    public GenerateFullStoryDraftAssetsGenerator(
        IBlobSasService blobSas,
        IGoogleImageService googleImage,
        IOpenAIImageWithApiKey openAIImage,
        ISequentialStoryImageGenerator sequentialImageGenerator,
        IGoogleAudioGeneratorService googleAudio,
        IOpenAIAudioWithApiKey openAIAudio,
        ILogger<GenerateFullStoryDraftAssetsGenerator> logger)
    {
        _blobSas = blobSas;
        _googleImage = googleImage;
        _openAIImage = openAIImage;
        _sequentialImageGenerator = sequentialImageGenerator;
        _googleAudio = googleAudio;
        _openAIAudio = openAIAudio;
        _logger = logger;
    }

    public async Task FillImagesAndAudioAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        bool isOpenAi,
        bool usePublishedPaths = false,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail))
            ownerEmail = "unknown";

        var storyJson = BuildMinimalStoryJson(dto.Title, dto.Summary);
        byte[]? imageSeedData = null;
        string? imageSeedMime = null;
        if (request.GenerateImages && !string.IsNullOrWhiteSpace(request.ImageSeedBase64))
        {
            try
            {
                imageSeedData = Convert.FromBase64String(request.ImageSeedBase64.Trim());
                imageSeedMime = request.ImageSeedMimeType?.Trim() ?? "image/png";
            }
            catch { /* ignore */ }
        }

        if (request.GenerateImages)
        {
            if (request.UseConsistentImageStyle)
                await FillImagesSequentialAsync(request, dto, storyId, ownerEmail, storyJson, imageSeedData, imageSeedMime, isOpenAi, usePublishedPaths, ct);
            else
                await FillImagesParallelAsync(request, dto, storyId, ownerEmail, storyJson, imageSeedData, imageSeedMime, isOpenAi, usePublishedPaths, ct);
        }

        if (request.GenerateAudio)
        {
            const int audioConcurrency = 5;
            var tilesWithText = dto.Tiles
                .Select((t, idx) => (Tile: t, Index: idx))
                .Where(x => !string.IsNullOrWhiteSpace(x.Tile.Text))
                .ToList();
            if (tilesWithText.Count > 0)
            {
                using var semaphore = new SemaphoreSlim(audioConcurrency, audioConcurrency);
                var lang = (request.LanguageCode ?? "").Trim().ToLowerInvariant();
                var container = usePublishedPaths ? _blobSas.PublishedContainer : _blobSas.DraftContainer;
                var tasks = tilesWithText.Select(async item =>
                {
                    await semaphore.WaitAsync(ct);
                    try
                    {
                        var tileText = (item.Tile.Text ?? string.Empty).Trim();
                        var (audioData, format) = isOpenAi
                            ? await _openAIAudio.GenerateAudioAsync(tileText, request.LanguageCode!, null, request.ApiKey.Trim(), request.AudioModel, ct)
                            : await _googleAudio.GenerateAudioAsync(tileText, request.LanguageCode!, null, null, request.ApiKey.Trim(), request.AudioModel, ct);
                        var ext = string.Equals(format, "mp3", StringComparison.OrdinalIgnoreCase) ? "mp3" : "wav";
                        var filename = $"{item.Tile.Id}.{ext}";
                        var asset = new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Audio, lang);
                        var blobPath = usePublishedPaths
                            ? StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId)
                            : StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);
                        await UploadBlobAsync(container, blobPath, audioData, ext == "mp3" ? "audio/mpeg" : "audio/wav", ct);
                        item.Tile.AudioUrl = usePublishedPaths ? blobPath : filename;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "GenerateFullStoryDraft: audio generation failed for tile {TileId}", item.Tile.Id);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                await Task.WhenAll(tasks);
            }
        }
    }

    private async Task FillImagesSequentialAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        string storyJson,
        byte[]? referenceImageData,
        string? referenceImageMime,
        bool isOpenAi,
        bool usePublishedPaths,
        CancellationToken ct)
    {
        if (isOpenAi)
        {
            await FillImagesSequentialOpenAiAsync(request, dto, storyId, ownerEmail, storyJson, referenceImageData, referenceImageMime, usePublishedPaths, ct);
            return;
        }

        // Google path: reuse shared sequential + reference chaining logic (same as Story Image Export).
        var tilesWithText = dto.Tiles
            .Select((t, idx) => (Tile: t, Index: idx, Text: (t.Text ?? string.Empty).Trim()))
            .Where(x => !string.IsNullOrEmpty(x.Text))
            .ToList();
        if (tilesWithText.Count == 0) return;

        var tileTexts = tilesWithText.Select(x => x.Text).ToList();
        var generatedImages = await _sequentialImageGenerator.GenerateSequentialWithChainingAsync(
            storyJson,
            tileTexts,
            request.LanguageCode ?? "en-us",
            request.ImageSeedInstructions,
            referenceImageData,
            referenceImageMime,
            request.ApiKey.Trim(),
            request.ImageModel,
            ct).ConfigureAwait(false);

        var container = usePublishedPaths ? _blobSas.PublishedContainer : _blobSas.DraftContainer;
        for (var j = 0; j < generatedImages.Count && j < tilesWithText.Count; j++)
        {
            var (imageData, mimeType) = generatedImages[j];
            var tile = tilesWithText[j].Tile;
            var originalIndex = tilesWithText[j].Index;
            var ext = (mimeType ?? "image/png").Contains("png", StringComparison.OrdinalIgnoreCase) ? "png" : "jpg";
            var filename = $"{tile.Id}.{ext}";
            var asset = new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Image, null);
            var blobPath = usePublishedPaths
                ? StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId)
                : StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);
            await UploadBlobAsync(container, blobPath, imageData, mimeType ?? "image/png", ct);
            tile.ImageUrl = usePublishedPaths ? blobPath : filename;
            if (originalIndex == 0) dto.CoverImageUrl = usePublishedPaths ? blobPath : filename;
        }
    }

    private async Task FillImagesSequentialOpenAiAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        string storyJson,
        byte[]? referenceImageData,
        string? referenceImageMime,
        bool usePublishedPaths,
        CancellationToken ct)
    {
        var container = usePublishedPaths ? _blobSas.PublishedContainer : _blobSas.DraftContainer;
        byte[]? previousImageData = referenceImageData;
        string? previousImageMime = referenceImageMime;
        for (var i = 0; i < dto.Tiles.Count; i++)
        {
            var tile = dto.Tiles[i];
            var tileText = (tile.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(tileText)) continue;
            try
            {
                var (imageData, mimeType) = await _openAIImage.GenerateStoryImageAsync(storyJson, tileText, request.LanguageCode, request.ImageSeedInstructions, previousImageData, previousImageMime, request.ApiKey.Trim(), request.ImageModel, request.ImageQuality?.Trim(), ct);
                var ext = (mimeType ?? "image/png").Contains("png", StringComparison.OrdinalIgnoreCase) ? "png" : "jpg";
                var filename = $"{tile.Id}.{ext}";
                var asset = new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Image, null);
                var blobPath = usePublishedPaths
                    ? StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId)
                    : StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);
                await UploadBlobAsync(container, blobPath, imageData, mimeType ?? "image/png", ct);
                tile.ImageUrl = usePublishedPaths ? blobPath : filename;
                previousImageData = imageData;
                previousImageMime = mimeType;
                if (i == 0) dto.CoverImageUrl = usePublishedPaths ? blobPath : filename;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GenerateFullStoryDraft: image generation failed for tile {TileId}", tile.Id);
            }
        }
    }

    private async Task FillImagesParallelAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        string storyJson,
        byte[]? referenceImageData,
        string? referenceImageMime,
        bool isOpenAi,
        bool usePublishedPaths,
        CancellationToken ct)
    {
        var tilesWithText = dto.Tiles
            .Select((t, idx) => (Tile: t, Index: idx))
            .Where(x => !string.IsNullOrWhiteSpace(x.Tile.Text))
            .ToList();
        if (tilesWithText.Count == 0) return;

        var tasks = tilesWithText.Select(async item =>
        {
            var (tile, index) = item;
            var tileText = (tile.Text ?? string.Empty).Trim();
            try
            {
                var (imageData, mimeType) = isOpenAi
                    ? await _openAIImage.GenerateStoryImageAsync(storyJson, tileText, request.LanguageCode, request.ImageSeedInstructions, referenceImageData, referenceImageMime, request.ApiKey.Trim(), request.ImageModel, request.ImageQuality?.Trim(), ct)
                    : await _googleImage.GenerateStoryImageAsync(storyJson, tileText, request.LanguageCode, request.ImageSeedInstructions, referenceImageData, referenceImageMime, ct, request.ApiKey.Trim(), request.ImageModel);
                return (item.Tile, item.Index, ImageData: imageData, MimeType: mimeType ?? "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GenerateFullStoryDraft: image generation failed for tile {TileId}", tile.Id);
                return (item.Tile, item.Index, (byte[]?)null, (string?)null);
            }
        });
        var results = await Task.WhenAll(tasks);

        var container = usePublishedPaths ? _blobSas.PublishedContainer : _blobSas.DraftContainer;
        foreach (var r in results.OrderBy(x => x.Index))
        {
            if (r.Item3 == null || r.Item4 == null) continue;
            var ext = r.Item4.Contains("png", StringComparison.OrdinalIgnoreCase) ? "png" : "jpg";
            var filename = $"{r.Tile.Id}.{ext}";
            var asset = new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Image, null);
            var blobPath = usePublishedPaths
                ? StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId)
                : StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);
            await UploadBlobAsync(container, blobPath, r.Item3, r.Item4, ct);
            r.Tile.ImageUrl = usePublishedPaths ? blobPath : filename;
            if (r.Index == 0) dto.CoverImageUrl = usePublishedPaths ? blobPath : filename;
        }
    }

    private static string BuildMinimalStoryJson(string? title, string? summary)
    {
        var obj = new Dictionary<string, string?>
        {
            ["title"] = title ?? "Story",
            ["summary"] = summary ?? string.Empty
        };
        return JsonSerializer.Serialize(obj);
    }

    private async Task UploadBlobAsync(string container, string blobPath, byte[] data, string contentType, CancellationToken ct)
    {
        var client = _blobSas.GetBlobClient(container, blobPath);
        using var stream = new MemoryStream(data);
        await client.UploadAsync(stream, overwrite: true, ct);
    }
}
