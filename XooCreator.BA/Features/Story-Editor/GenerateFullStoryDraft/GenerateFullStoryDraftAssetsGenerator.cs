using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Models;
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
    /// <param name="storyBible">Optional Story Bible for improved character consistency in image prompts.</param>
    /// <param name="scenePlan">Optional scene plan with detailed scene definitions.</param>
    /// <param name="promptBuilder">Optional prompt builder for Story Bible-based prompts.</param>
    Task FillImagesAndAudioAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        bool isOpenAi,
        bool usePublishedPaths = false,
        CancellationToken ct = default,
        StoryBible? storyBible = null,
        ScenePlan? scenePlan = null,
        IIllustrationPromptBuilder? promptBuilder = null);
}

public sealed class GenerateFullStoryDraftAssetsGenerator : IGenerateFullStoryDraftAssetsGenerator
{
    private readonly IBlobSasService _blobSas;
    private readonly IGoogleImageService _googleImage;
    private readonly IOpenAIImageWithApiKey _openAIImage;
    private readonly ISequentialStoryImageGenerator _sequentialImageGenerator;
    private readonly IGoogleAudioGeneratorService _googleAudio;
    private readonly IOpenAIAudioWithApiKey _openAIAudio;
    private readonly IDiacriticsNormalizer _diacriticsNormalizer;
    private readonly IStoryImagePromptConsistencyValidator _promptConsistencyValidator;
    private readonly ICharacterPresenceResolver _characterPresenceResolver;
    private readonly ILogger<GenerateFullStoryDraftAssetsGenerator> _logger;

    public GenerateFullStoryDraftAssetsGenerator(
        IBlobSasService blobSas,
        IGoogleImageService googleImage,
        IOpenAIImageWithApiKey openAIImage,
        ISequentialStoryImageGenerator sequentialImageGenerator,
        IGoogleAudioGeneratorService googleAudio,
        IOpenAIAudioWithApiKey openAIAudio,
        IDiacriticsNormalizer diacriticsNormalizer,
        IStoryImagePromptConsistencyValidator promptConsistencyValidator,
        ICharacterPresenceResolver characterPresenceResolver,
        ILogger<GenerateFullStoryDraftAssetsGenerator> logger)
    {
        _blobSas = blobSas;
        _googleImage = googleImage;
        _openAIImage = openAIImage;
        _sequentialImageGenerator = sequentialImageGenerator;
        _googleAudio = googleAudio;
        _openAIAudio = openAIAudio;
        _diacriticsNormalizer = diacriticsNormalizer;
        _promptConsistencyValidator = promptConsistencyValidator;
        _characterPresenceResolver = characterPresenceResolver;
        _logger = logger;
    }

    public async Task FillImagesAndAudioAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        bool isOpenAi,
        bool usePublishedPaths = false,
        CancellationToken ct = default,
        StoryBible? storyBible = null,
        ScenePlan? scenePlan = null,
        IIllustrationPromptBuilder? promptBuilder = null)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail))
            ownerEmail = "unknown";

        // Use Story Bible-enhanced context if available
        var storyJson = storyBible != null 
            ? BuildStoryBibleContext(storyBible, dto.Title, dto.Summary, dto.Tiles, fullContext: true)
            : BuildMinimalStoryJson(dto.Title, dto.Summary);
            
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
            // Use Story Bible pipeline if available
            if (storyBible != null && promptBuilder != null && (scenePlan != null || usePublishedPaths))
            {
                await FillImagesWithStoryBibleAsync(request, dto, storyId, ownerEmail, storyBible, scenePlan, promptBuilder, imageSeedData, imageSeedMime, isOpenAi, usePublishedPaths, ct);
            }
            else if (request.UseConsistentImageStyle)
            {
                await FillImagesSequentialAsync(request, dto, storyId, ownerEmail, storyJson, imageSeedData, imageSeedMime, isOpenAi, usePublishedPaths, ct);
            }
            else
            {
                await FillImagesParallelAsync(request, dto, storyId, ownerEmail, storyJson, imageSeedData, imageSeedMime, isOpenAi, usePublishedPaths, ct);
            }
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
                        var normalizedText = await NormalizeForAudioAsync(tileText, request, ct);
                        var (audioData, format) = isOpenAi
                            ? await _openAIAudio.GenerateAudioAsync(normalizedText, request.LanguageCode!, null, request.ApiKey.Trim(), request.AudioModel, ct)
                            : await _googleAudio.GenerateAudioAsync(normalizedText, request.LanguageCode!, null, null, request.ApiKey.Trim(), request.AudioModel, ct);
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

        if (usePublishedPaths)
        {
            await FillImagesSequentialGoogleResilientPrivateAsync(
                request,
                dto,
                storyId,
                ownerEmail,
                storyJson,
                referenceImageData,
                referenceImageMime,
                usePublishedPaths,
                ct);
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

    private async Task FillImagesSequentialGoogleResilientPrivateAsync(
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
        byte[]? currentRefBytes = referenceImageData;
        string? currentRefMime = referenceImageMime;

        for (var i = 0; i < dto.Tiles.Count; i++)
        {
            var tile = dto.Tiles[i];
            var tileText = (tile.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tileText))
                continue;

            try
            {
                _logger.LogInformation(
                    "Private image generation attempt. storyId={StoryId} tileId={TileId} index={Index} attempt={Attempt} fallbackUsed={FallbackUsed}",
                    storyId,
                    tile.Id,
                    i,
                    1,
                    false);

                var (imageData, mimeType) = await GenerateGoogleImageWithPromptFallbackAsync(
                    storyJson,
                    tileText,
                    BuildGenericImageFallbackPrompt(tileText, request.LanguageCode ?? "ro-ro"),
                    request.LanguageCode ?? "en-us",
                    request.ImageSeedInstructions,
                    currentRefBytes,
                    currentRefMime,
                    request.ApiKey.Trim(),
                    request.ImageModel,
                    storyId,
                    tile.Id,
                    i,
                    ct);

                var ext = (mimeType ?? "image/png").Contains("png", StringComparison.OrdinalIgnoreCase) ? "png" : "jpg";
                var filename = $"{tile.Id}.{ext}";
                var asset = new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Image, null);
                var blobPath = usePublishedPaths
                    ? StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId)
                    : StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);

                await UploadBlobAsync(container, blobPath, imageData, mimeType ?? "image/png", ct);
                tile.ImageUrl = usePublishedPaths ? blobPath : filename;
                currentRefBytes = imageData;
                currentRefMime = mimeType;

                if (i == 0)
                    dto.CoverImageUrl = usePublishedPaths ? blobPath : filename;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Private resilient image generation failed. storyId={StoryId} tileId={TileId} index={Index}. Skipping tile image.",
                    storyId,
                    tile.Id,
                    i);
            }
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
                return (Tile: item.Tile, Index: item.Index, ImageData: imageData, MimeType: mimeType ?? "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GenerateFullStoryDraft: image generation failed for tile {TileId}", tile.Id);
                return (Tile: item.Tile, Index: item.Index, ImageData: (byte[]?)null, MimeType: (string?)null);
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

    /// <summary>
    /// Generate images using Story Bible pipeline for improved character consistency.
    /// </summary>
    private async Task FillImagesWithStoryBibleAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        StoryBible storyBible,
        ScenePlan? scenePlan,
        IIllustrationPromptBuilder promptBuilder,
        byte[]? referenceImageData,
        string? referenceImageMime,
        bool isOpenAi,
        bool usePublishedPaths,
        CancellationToken ct)
    {
        var container = usePublishedPaths ? _blobSas.PublishedContainer : _blobSas.DraftContainer;
        byte[]? currentRefBytes = referenceImageData;
        string? currentRefMime = referenceImageMime;
        var expectedAnchorsTotal = 0;
        var patchedAnchorsTotal = 0;
        var languageCode = request.LanguageCode ?? "en-us";

        // Match tiles with scenes
        var tilesWithScenes = dto.Tiles
            .Select((tile, idx) => (Tile: tile, Index: idx, Scene: scenePlan != null && idx < scenePlan.Scenes.Count ? scenePlan.Scenes[idx] : null))
            .Where(x => !string.IsNullOrWhiteSpace(x.Tile.Text))
            .ToList();

        for (var i = 0; i < tilesWithScenes.Count; i++)
        {
            var (tile, index, scene) = tilesWithScenes[i];
            
            try
            {
                // Build prompt using Story Bible context
                string promptText;
                if (scene != null)
                {
                    var (mergedScene, wasPatchedByHints) = await MergeCharactersPresentWithTextHintsAsync(
                        scene,
                        storyBible,
                        tile.Text ?? string.Empty,
                        request.ApiKey.Trim(),
                        request.TextModel,
                        ct);
                    if (wasPatchedByHints)
                    {
                        _logger.LogInformation(
                            "Patched scene characters from text hints. storyId={StoryId} sceneId={SceneId} chars={Characters}",
                            storyId,
                            mergedScene.SceneId,
                            string.Join(",", mergedScene.CharactersPresent));
                    }

                    var illustrationPrompt = promptBuilder.Build(storyBible, mergedScene);
                    var lockedPrompt = AddHardRosterLock(promptBuilder.GetPromptText(illustrationPrompt), storyBible);
                    expectedAnchorsTotal += illustrationPrompt.CharacterAnchors.Count;
                    var validation = _promptConsistencyValidator.ValidateAndPatch(
                        lockedPrompt,
                        storyBible,
                        mergedScene,
                        illustrationPrompt.CharacterAnchors);
                    promptText = validation.PromptText;
                    patchedAnchorsTotal += validation.PatchedCount;
                }
                else
                {
                    // Fallback: use tile text with character descriptions
                    promptText = AddHardRosterLock(
                        BuildFallbackPromptWithBible(storyBible, tile.Text ?? string.Empty),
                        storyBible);
                    var validation = _promptConsistencyValidator.ValidateAndPatch(
                        promptText,
                        storyBible,
                        null,
                        []);
                    promptText = validation.PromptText;
                    patchedAnchorsTotal += validation.PatchedCount;
                }

                _logger.LogInformation(
                    "{ColoredProgress}",
                    ColoredLogHelper.FormatProgress(i + 1, tilesWithScenes.Count, $"tile={tile.Id}"));

                var promptForGeneration = BuildPromptWithReferenceInstruction(promptText, currentRefBytes != null && currentRefBytes.Length > 0);
                var (imageData, mimeType) = isOpenAi
                    ? await _openAIImage.GenerateStoryImageAsync(
                        BuildStoryBibleContext(storyBible, dto.Title, dto.Summary, dto.Tiles, fullContext: true),
                        promptForGeneration,
                        languageCode,
                        request.ImageSeedInstructions,
                        currentRefBytes,
                        currentRefMime,
                        request.ApiKey.Trim(),
                        request.ImageModel,
                        request.ImageQuality?.Trim(),
                        ct)
                    : await GenerateGoogleImageFromBuiltPromptWithFallbackAsync(
                        promptForGeneration,
                        BuildGenericImageFallbackPrompt(tile.Text ?? string.Empty, request.LanguageCode ?? "ro-ro"),
                        currentRefBytes,
                        currentRefMime,
                        request.ApiKey.Trim(),
                        request.ImageModel,
                        storyId,
                        tile.Id,
                        index,
                        ct);

                var ext = (mimeType ?? "image/png").Contains("png", StringComparison.OrdinalIgnoreCase) ? "png" : "jpg";
                var filename = $"{tile.Id}.{ext}";
                var asset = new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Image, null);
                var blobPath = usePublishedPaths
                    ? StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId)
                    : StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);

                await UploadBlobAsync(container, blobPath, imageData, mimeType ?? "image/png", ct);
                tile.ImageUrl = usePublishedPaths ? blobPath : filename;

                // Use this image as reference for next (chaining for consistency)
                currentRefBytes = imageData;
                currentRefMime = mimeType;

                if (index == 0)
                    dto.CoverImageUrl = usePublishedPaths ? blobPath : filename;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Story Bible image generation failed for tile {TileId}, index {Index}", tile.Id, index);
            }
        }

        var anchorCoveragePercent = expectedAnchorsTotal == 0
            ? 100
            : Math.Max(0, Math.Min(100, (int)Math.Round(((double)(expectedAnchorsTotal - patchedAnchorsTotal) / expectedAnchorsTotal) * 100)));
        _logger.LogInformation(
            "Story Bible image consistency stats: storyId={StoryId} expectedAnchors={ExpectedAnchors} patched={Patched} coveragePercent={CoveragePercent}",
            storyId,
            expectedAnchorsTotal,
            patchedAnchorsTotal,
            anchorCoveragePercent);
    }

    /// <summary>
    /// Build enhanced story context including character descriptions from Story Bible.
    /// </summary>
    private static string BuildStoryBibleContext(
        StoryBible bible,
        string? title,
        string? summary,
        IReadOnlyList<EditableTileDto>? tiles = null,
        bool fullContext = false)
    {
        var characters = bible.Characters.Select(c => new
        {
            c.Name,
            c.Species,
            Visual = $"{c.Visual.PrimaryColor} {c.Visual.Size}, features: {string.Join(", ", c.Visual.Features)}",
            Marker = c.Visual.Accessories.Count > 0 ? string.Join(", ", c.Visual.Accessories) : null
        });

        var pages = tiles?
            .Where(t => !string.IsNullOrWhiteSpace(t.Text))
            .Select((t, idx) => new
            {
                index = idx + 1,
                text = t.Text
            })
            .ToList();

        var obj = new
        {
            title = title ?? bible.Title,
            summary = summary ?? string.Empty,
            visualStyle = bible.VisualStyle,
            setting = $"{bible.Setting.Place}, {bible.Setting.Time}",
            characters,
            fullContext,
            pages
        };

        return JsonSerializer.Serialize(obj);
    }

    private async Task<(SceneDefinition Scene, bool WasPatched)> MergeCharactersPresentWithTextHintsAsync(
        SceneDefinition scene,
        StoryBible bible,
        string tileText,
        string apiKey,
        string? model,
        CancellationToken ct)
    {
        var wasPatched = false;
        var existing = NormalizeCharacterReferences(scene.CharactersPresent, bible);
        var text = tileText.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return (scene with { CharactersPresent = existing.ToList() }, false);

        // Deterministic layer: add direct character name mentions from page text.
        foreach (var character in bible.Characters)
        {
            var nameMentioned = text.Contains(character.Name, StringComparison.OrdinalIgnoreCase);
            if (nameMentioned && !existing.Contains(character.Id))
            {
                existing.Add(character.Id);
                wasPatched = true;
            }
        }

        // AI layer: infer additional present characters without language-specific hardcoding.
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            try
            {
                var inferredCharacterIds = await _characterPresenceResolver.ResolveAsync(
                    bible,
                    scene,
                    text,
                    apiKey,
                    model,
                    ct);

                foreach (var inferredId in inferredCharacterIds)
                {
                    if (!existing.Contains(inferredId))
                    {
                        existing.Add(inferredId);
                        wasPatched = true;
                    }
                }
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogWarning(
                    ex,
                    "Character presence AI resolver failed. sceneId={SceneId}. Continuing with deterministic character matches.",
                    scene.SceneId);
            }
        }

        if (!wasPatched)
            return (scene with { CharactersPresent = existing.ToList() }, false);

        return (scene with { CharactersPresent = existing.ToList() }, true);
    }

    private static HashSet<string> NormalizeCharacterReferences(IEnumerable<string> references, StoryBible bible)
    {
        var normalized = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var reference in references.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            var character = bible.Characters.FirstOrDefault(c =>
                c.Id.Equals(reference, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Equals(reference, StringComparison.OrdinalIgnoreCase));

            normalized.Add(character?.Id ?? reference);
        }

        return normalized;
    }

    private static string AddHardRosterLock(string promptText, StoryBible bible)
    {
        var roster = bible.Characters.Select(c =>
        {
            var marker = c.Visual.Accessories.Count > 0 ? string.Join(", ", c.Visual.Accessories) : "no-marker";
            return $"- {c.Name} ({c.Species}): color={c.Visual.PrimaryColor}; size={c.Visual.Size}; marker={marker}; features={string.Join(", ", c.Visual.Features)}";
        });

        return $@"{promptText}

CHARACTER ROSTER LOCK (IMMUTABLE FOR ALL PAGES):
{string.Join(Environment.NewLine, roster)}

LOCK RULES:
- Keep character identities stable across all pages.
- Do not swap colors/markers between characters.
- If scene text references a character pair or group, include the matching recurring characters.";
    }

    /// <summary>
    /// Build a fallback prompt with character descriptions when scene is not available.
    /// </summary>
    private static string BuildFallbackPromptWithBible(StoryBible bible, string tileText)
    {
        var characterDescriptions = string.Join("\n", bible.Characters.Select(c =>
            $"- {c.Name}: {c.Visual.PrimaryColor} {c.Visual.Size} {c.Species}, {string.Join(", ", c.Visual.Features)}"));

        return $@"Children's book illustration. Colorful, friendly. No text. Portrait, vertical composition.
Visual style: {bible.VisualStyle}
Setting: {bible.Setting.Place}, {bible.Setting.Time}

Characters (maintain EXACT appearance):
{characterDescriptions}

Scene: {tileText}

CONSISTENCY RULES: Do not change character colors, sizes, or species.";
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

    /// <summary>
    /// Best-effort diacritics normalization before sending text to TTS.
    /// On any error, returns the original text.
    /// </summary>
    private async Task<string> NormalizeForAudioAsync(
        string tileText,
        GenerateFullStoryDraftRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tileText))
            return tileText;

        var languageCode = request.LanguageCode ?? string.Empty;

        try
        {
            return await _diacriticsNormalizer.NormalizeAsync(
                tileText,
                languageCode,
                request.ApiKey.Trim(),
                request.TextModel,
                ct);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "Diacritics normalization failed for audio. Falling back to original text. Lang={LanguageCode}, Length={Length}",
                languageCode,
                tileText.Length);
            return tileText;
        }
    }

    private async Task UploadBlobAsync(string container, string blobPath, byte[] data, string contentType, CancellationToken ct)
    {
        var client = _blobSas.GetBlobClient(container, blobPath);
        using var stream = new MemoryStream(data);
        await client.UploadAsync(stream, overwrite: true, ct);
    }

    private async Task<(byte[] ImageData, string MimeType)> GenerateGoogleImageWithReferenceFallbackAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions,
        byte[]? referenceImageData,
        string? referenceImageMime,
        string apiKey,
        string? imageModel,
        CancellationToken ct)
    {
        try
        {
            return await _googleImage.GenerateStoryImageAsync(
                storyJson,
                tileText,
                languageCode,
                extraInstructions,
                referenceImageData,
                referenceImageMime,
                ct,
                apiKey,
                imageModel);
        }
        catch (GoogleImageGenerationException ex) when (
            referenceImageData is { Length: > 0 } &&
            ShouldRetryWithoutReference(ex))
        {
            _logger.LogWarning(
                "{ColoredStatus}",
                ColoredLogHelper.FormatImageGeneration("gemini-image", "RETRY", $"dropping ref image, reason={ex.FinishReason}"));

            return await _googleImage.GenerateStoryImageAsync(
                storyJson,
                tileText,
                languageCode,
                extraInstructions,
                referenceImage: null,
                referenceImageMimeType: null,
                ct: ct,
                apiKeyOverride: apiKey,
                modelOverride: imageModel);
        }
    }

    private static bool ShouldRetryWithoutReference(GoogleImageGenerationException ex)
    {
        if (ex.IsTransient)
            return true;

        return string.Equals(ex.FinishReason, "IMAGE_OTHER", StringComparison.OrdinalIgnoreCase)
               || string.Equals(ex.ErrorCode, "GoogleImageUnexpectedFinishReason", StringComparison.OrdinalIgnoreCase)
               || string.Equals(ex.ErrorCode, "GoogleImageNoInlineData", StringComparison.OrdinalIgnoreCase)
               || string.Equals(ex.ErrorCode, "GoogleImageMissingParts", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<(byte[] ImageData, string MimeType)> GenerateGoogleImageWithPromptFallbackAsync(
        string storyJson,
        string primaryPrompt,
        string genericFallbackPrompt,
        string languageCode,
        string? extraInstructions,
        byte[]? referenceImageData,
        string? referenceImageMime,
        string apiKey,
        string? imageModel,
        string storyId,
        string tileId,
        int tileIndex,
        CancellationToken ct)
    {
        try
        {
            return await GenerateGoogleImageWithReferenceFallbackAsync(
                storyJson,
                primaryPrompt,
                languageCode,
                extraInstructions,
                referenceImageData,
                referenceImageMime,
                apiKey,
                imageModel,
                ct);
        }
        catch (GoogleImageGenerationException ex) when (ShouldAttemptGenericFallback(ex))
        {
            _logger.LogWarning(
                "Primary image prompt failed, trying generic fallback. storyId={StoryId} tileId={TileId} index={TileIndex} attempt={Attempt} finishReason={FinishReason} errorCode={ErrorCode} fallbackUsed={FallbackUsed}",
                storyId,
                tileId,
                tileIndex,
                2,
                ex.FinishReason,
                ex.ErrorCode,
                true);

            return await GenerateGoogleImageWithReferenceFallbackAsync(
                storyJson,
                genericFallbackPrompt,
                languageCode,
                extraInstructions,
                referenceImageData: null,
                referenceImageMime: null,
                apiKey,
                imageModel,
                ct);
        }
    }

    private async Task<(byte[] ImageData, string MimeType)> GenerateGoogleImageFromBuiltPromptWithFallbackAsync(
        string primaryPrompt,
        string genericFallbackPrompt,
        byte[]? referenceImageData,
        string? referenceImageMime,
        string apiKey,
        string? imageModel,
        string storyId,
        string tileId,
        int tileIndex,
        CancellationToken ct)
    {
        try
        {
            return await _googleImage.GenerateFromBuiltPromptAsync(
                primaryPrompt,
                referenceImageData,
                referenceImageMime,
                ct,
                apiKey,
                imageModel);
        }
        catch (GoogleImageGenerationException ex) when (
            referenceImageData is { Length: > 0 } &&
            ShouldRetryWithoutReference(ex))
        {
            _logger.LogWarning(
                "{ColoredStatus}",
                ColoredLogHelper.FormatImageGeneration("gemini-image", "RETRY", $"dropping ref image, reason={ex.FinishReason}"));

            try
            {
                return await _googleImage.GenerateFromBuiltPromptAsync(
                    primaryPrompt,
                    referenceImage: null,
                    referenceImageMimeType: null,
                    ct: ct,
                    apiKeyOverride: apiKey,
                    modelOverride: imageModel);
            }
            catch (GoogleImageGenerationException innerEx) when (ShouldAttemptGenericFallback(innerEx))
            {
                _logger.LogWarning(
                    "Primary built prompt failed after ref-drop retry, trying generic fallback. storyId={StoryId} tileId={TileId} index={TileIndex} finishReason={FinishReason} errorCode={ErrorCode}",
                    storyId,
                    tileId,
                    tileIndex,
                    innerEx.FinishReason,
                    innerEx.ErrorCode);

                return await _googleImage.GenerateFromBuiltPromptAsync(
                    genericFallbackPrompt,
                    referenceImage: null,
                    referenceImageMimeType: null,
                    ct: ct,
                    apiKeyOverride: apiKey,
                    modelOverride: imageModel);
            }
        }
        catch (GoogleImageGenerationException ex) when (ShouldAttemptGenericFallback(ex))
        {
            _logger.LogWarning(
                "Primary built prompt failed, trying generic fallback. storyId={StoryId} tileId={TileId} index={TileIndex} finishReason={FinishReason} errorCode={ErrorCode}",
                storyId,
                tileId,
                tileIndex,
                ex.FinishReason,
                ex.ErrorCode);

            return await _googleImage.GenerateFromBuiltPromptAsync(
                genericFallbackPrompt,
                referenceImage: null,
                referenceImageMimeType: null,
                ct: ct,
                apiKeyOverride: apiKey,
                modelOverride: imageModel);
        }
    }

    private static string BuildPromptWithReferenceInstruction(string promptText, bool hasReferenceImage)
    {
        if (!hasReferenceImage)
            return promptText;

        return $"{promptText}{Environment.NewLine}{Environment.NewLine}REFERENCE IMAGE: The attached image defines established character appearance and art style.{Environment.NewLine}Match character colors, proportions, features, and accessories EXACTLY as shown.";
    }

    private static bool ShouldAttemptGenericFallback(GoogleImageGenerationException ex)
    {
        if (ex.IsTransient)
            return true;

        return string.Equals(ex.FinishReason, "IMAGE_OTHER", StringComparison.OrdinalIgnoreCase)
               || string.Equals(ex.ErrorCode, "GoogleImageMissingContent", StringComparison.OrdinalIgnoreCase)
               || string.Equals(ex.ErrorCode, "GoogleImageNoInlineData", StringComparison.OrdinalIgnoreCase)
               || string.Equals(ex.ErrorCode, "GoogleImageMissingParts", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildGenericImageFallbackPrompt(string tileText, string languageCode)
    {
        return
            "Children's book illustration, safe and friendly, no text, no nudity, no sexual content, no graphic violence. " +
            "Use a simple composition, bright colors, and age-appropriate tone. " +
            $"Language context: {languageCode}. " +
            $"Scene summary: {tileText}";
    }
}
