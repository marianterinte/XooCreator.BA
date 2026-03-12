using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
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
            if (storyBible != null && scenePlan != null && promptBuilder != null)
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

    /// <summary>
    /// Generate images using Story Bible pipeline for improved character consistency.
    /// </summary>
    private async Task FillImagesWithStoryBibleAsync(
        GenerateFullStoryDraftRequest request,
        EditableStoryDto dto,
        string storyId,
        string ownerEmail,
        StoryBible storyBible,
        ScenePlan scenePlan,
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
        var storyBibleContext = BuildStoryBibleContext(storyBible, dto.Title, dto.Summary, dto.Tiles, fullContext: true);

        // Match tiles with scenes
        var tilesWithScenes = dto.Tiles
            .Select((tile, idx) => (Tile: tile, Index: idx, Scene: idx < scenePlan.Scenes.Count ? scenePlan.Scenes[idx] : null))
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
                    var mergedScene = MergeCharactersPresentWithTextHints(scene, storyBible, tile.Text ?? string.Empty, out var wasPatchedByHints);
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

                _logger.LogDebug("Generating image {Index} with Story Bible prompt", index + 1);

                var (imageData, mimeType) = isOpenAi
                    ? await _openAIImage.GenerateStoryImageAsync(
                        storyBibleContext,
                        promptText,
                        request.LanguageCode,
                        request.ImageSeedInstructions,
                        currentRefBytes,
                        currentRefMime,
                        request.ApiKey.Trim(),
                        request.ImageModel,
                        request.ImageQuality?.Trim(),
                        ct)
                    : await GenerateGoogleImageWithReferenceFallbackAsync(
                        storyBibleContext,
                        promptText,
                        request.LanguageCode,
                        request.ImageSeedInstructions,
                        currentRefBytes,
                        currentRefMime,
                        request.ApiKey.Trim(),
                        request.ImageModel,
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

    private static SceneDefinition MergeCharactersPresentWithTextHints(
        SceneDefinition scene,
        StoryBible bible,
        string tileText,
        out bool wasPatched)
    {
        wasPatched = false;
        var existing = new HashSet<string>(scene.CharactersPresent, StringComparer.OrdinalIgnoreCase);
        var text = tileText.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return scene;

        foreach (var character in bible.Characters)
        {
            var nameMentioned = text.Contains(character.Name, StringComparison.OrdinalIgnoreCase);
            var speciesMentioned = IsSpeciesMentioned(text, character.Species);

            if ((nameMentioned || speciesMentioned) && !existing.Contains(character.Id) && !existing.Contains(character.Name))
            {
                existing.Add(character.Id);
                wasPatched = true;
            }
        }

        if (!wasPatched)
            return scene;

        return scene with { CharactersPresent = existing.ToList() };
    }

    private static bool IsSpeciesMentioned(string text, string species)
    {
        if (string.IsNullOrWhiteSpace(species))
            return false;

        var s = species.Trim().ToLowerInvariant();
        if (text.Contains(s, StringComparison.OrdinalIgnoreCase))
            return true;

        return s switch
        {
            "cat" => text.Contains("cats", StringComparison.OrdinalIgnoreCase)
                || text.Contains("pisica", StringComparison.OrdinalIgnoreCase)
                || text.Contains("pisici", StringComparison.OrdinalIgnoreCase)
                || text.Contains("pisicile", StringComparison.OrdinalIgnoreCase),
            "dog" => text.Contains("dogs", StringComparison.OrdinalIgnoreCase)
                || text.Contains("caine", StringComparison.OrdinalIgnoreCase)
                || text.Contains("caini", StringComparison.OrdinalIgnoreCase),
            "duck" => text.Contains("ducks", StringComparison.OrdinalIgnoreCase)
                || text.Contains("rata", StringComparison.OrdinalIgnoreCase)
                || text.Contains("ratusca", StringComparison.OrdinalIgnoreCase)
                || text.Contains("ratuste", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
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
- If scene text references a species pair (e.g. two cats), include the matching recurring characters.";
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
                ex,
                "Google image generation failed with reference image (code={ErrorCode}, finishReason={FinishReason}). Retrying once without reference image.",
                ex.ErrorCode,
                ex.FinishReason);

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
}
