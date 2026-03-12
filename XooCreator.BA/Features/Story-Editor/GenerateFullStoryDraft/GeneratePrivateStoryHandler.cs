using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Generates private story: text via Google (server key), images/audio via assets generator (published paths), then creates StoryDefinition with IsPrivate.
/// </summary>
public sealed class GeneratePrivateStoryHandler : IGeneratePrivateStoryHandler
{
    private const int MaxIdeaLength = 2000;
    private const int MinPageCount = 5;
    private const int MaxPageCount = 10;
    private const decimal StoryBibleCostPerCallUsd = 0.002m;
    private const decimal StoryTextCostPerCallUsd = 0.002m;
    private const decimal ScenePlanCostPerCallUsd = 0.001m;
    private const decimal CharacterReferenceSheetCostPerCallUsd = 0.04m;
    private const decimal StoryImageCostPerCallUsd = 0.04m;
    private const decimal StoryAudioCostPerCallUsd = 0.015m;
    private const decimal DiacriticsCostPerCallUsd = 0.0005m;
    private static readonly Regex UnsafePromptPattern = new(
        @"\b(porn|pornograf|porno|nudity|nude|explicit sexual|sex explicit|violenta sexuala|abuz sexual|sex cu minori|child sexual|child porn|rape|gore|snuff)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly IGoogleTextService _googleTextService;
    private readonly IGoogleImageService _googleImageService;
    private readonly IGenerateFullStoryDraftAssetsGenerator _assetsGenerator;
    private readonly IPrivateStoryCreationService _privateStoryCreation;
    private readonly IStoryBibleGenerator _storyBibleGenerator;
    private readonly IScenePlanner _scenePlanner;
    private readonly IIllustrationPromptBuilder _illustrationPromptBuilder;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeneratePrivateStoryHandler> _logger;

    public GeneratePrivateStoryHandler(
        IStoryIdGenerator storyIdGenerator,
        IGoogleTextService googleTextService,
        IGoogleImageService googleImageService,
        IGenerateFullStoryDraftAssetsGenerator assetsGenerator,
        IPrivateStoryCreationService privateStoryCreation,
        IStoryBibleGenerator storyBibleGenerator,
        IScenePlanner scenePlanner,
        IIllustrationPromptBuilder illustrationPromptBuilder,
        IConfiguration configuration,
        ILogger<GeneratePrivateStoryHandler> logger)
    {
        _storyIdGenerator = storyIdGenerator;
        _googleTextService = googleTextService;
        _googleImageService = googleImageService;
        _assetsGenerator = assetsGenerator;
        _privateStoryCreation = privateStoryCreation;
        _storyBibleGenerator = storyBibleGenerator;
        _scenePlanner = scenePlanner;
        _illustrationPromptBuilder = illustrationPromptBuilder;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GeneratePrivateStoryResponse> ExecuteAsync(
        GeneratePrivateStoryRequest request,
        Guid ownerUserId,
        string ownerFirstName,
        string ownerLastName,
        string ownerEmail,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Idea) || request.Idea.Trim().Length < 20)
            throw new ArgumentException("Idea must be at least 20 characters.", nameof(request));
        if (request.Idea.Trim().Length > MaxIdeaLength)
            throw new ArgumentException($"Idea must be at most {MaxIdeaLength} characters.", nameof(request));
        ValidateChildSafetyOrThrow(request.Idea);

        var storyId = await _storyIdGenerator.GenerateNextAsync(ownerUserId, ownerFirstName, ownerLastName, ct);
        var lang = (request.LanguageCode ?? "ro-ro").Trim().ToLowerInvariant();
        var rawPageCount = request.PageCount <= 0 ? MinPageCount : request.PageCount;
        var pageCount = Math.Clamp(rawPageCount, MinPageCount, MaxPageCount);
        _logger.LogInformation(
            "GeneratePrivateStory: storyId={StoryId} lang={Lang} pages={Pages} images={GenerateImages} audio={GenerateAudio}",
            storyId,
            lang,
            pageCount,
            request.GenerateImages,
            request.GenerateAudio);

        var apiKey = _configuration["GoogleAI:ApiKey"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("GoogleAI:ApiKey is not configured for private story generation.");

        StoryBible? storyBible = null;
        ScenePlan? scenePlan = null;
        var attemptedStoryBible = false;
        var attemptedScenePlan = false;
        var attemptedCharacterRefSheet = false;

        try
        {
            attemptedStoryBible = true;
            storyBible = await RetryAsync(
                () => _storyBibleGenerator.GenerateAsync(
                    request.Idea.Trim(),
                    request.Title?.Trim(),
                    pageCount,
                    lang,
                    apiKey,
                    model: null,
                    ct),
                "private-story-bible",
                ct);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            storyBible = null;
            _logger.LogWarning(ex, "Private story Story Bible generation failed for storyId={StoryId}. Falling back to classic prompts.", storyId);
        }

        string rawText;
        if (storyBible != null)
        {
            try
            {
                rawText = await GenerateStoryTextWithBibleAsync(storyBible, pageCount, lang, apiKey, ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Private story Story Bible text generation failed for storyId={StoryId}. Falling back to classic prompts.", storyId);
                rawText = await GenerateStoryTextClassicAsync(request.Idea.Trim(), request.Title?.Trim(), pageCount, lang, apiKey, ct);
            }
        }
        else
        {
            rawText = await GenerateStoryTextClassicAsync(request.Idea.Trim(), request.Title?.Trim(), pageCount, lang, apiKey, ct);
        }

        var parseResult = StoryTextDelimiterParser.Parse(rawText);
        var dto = BuildEditableStoryDto(storyId, lang, parseResult);

        if (request.GenerateImages && storyBible != null)
        {
            try
            {
                var pageTexts = parseResult.Pages.Select(p => p.Text).ToList();
                attemptedScenePlan = true;
                scenePlan = await RetryAsync(
                    () => _scenePlanner.PlanAsync(
                        storyBible,
                        pageTexts,
                        storyId,
                        apiKey,
                        model: null,
                        ct),
                    "private-story-scene-plan",
                    ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                scenePlan = null;
                _logger.LogWarning(ex, "Private story scene planning failed for storyId={StoryId}. Falling back to sequential prompts.", storyId);
            }
        }

        byte[]? characterSeedImageData = null;
        string? characterSeedMimeType = null;
        if (request.GenerateImages && storyBible != null)
        {
            attemptedCharacterRefSheet = true;
            var seedResult = await TryGenerateCharacterReferenceSeedAsync(storyBible, apiKey, ct);
            characterSeedImageData = seedResult.ImageData;
            characterSeedMimeType = seedResult.MimeType;
        }

        var syntheticRequest = new GenerateFullStoryDraftRequest
        {
            ApiKey = apiKey,
            Provider = "Google",
            TextSeed = request.Idea.Trim(),
            NumberOfPages = pageCount,
            LanguageCode = lang,
            GenerateImages = request.GenerateImages,
            GenerateAudio = request.GenerateAudio,
            UseConsistentImageStyle = true,
            UseStoryBible = storyBible != null,
            AudioModel = "gemini-2.5-pro-preview-tts",
            Title = request.Title,
            ImageSeedBase64 = characterSeedImageData != null ? Convert.ToBase64String(characterSeedImageData) : null,
            ImageSeedMimeType = characterSeedMimeType
        };

        if (request.GenerateImages || request.GenerateAudio)
        {
            await RetryAsync(
                () => _assetsGenerator.FillImagesAndAudioAsync(
                    syntheticRequest,
                    dto,
                    storyId,
                    ownerEmail ?? "unknown",
                    isOpenAi: false,
                    usePublishedPaths: true,
                    ct,
                    storyBible,
                    scenePlan,
                    _illustrationPromptBuilder),
                "private-story-assets",
                ct);
        }

        var warnings = CollectGenerationWarnings(dto, request.GenerateImages, request.GenerateAudio);
        await _privateStoryCreation.CreateFromDtoAsync(dto, storyId, ownerUserId, ownerEmail ?? string.Empty, lang, ct);
        LogEstimatedStoryCost(
            storyId,
            dto,
            request,
            attemptedStoryBible,
            storyBible != null,
            attemptedScenePlan,
            scenePlan != null,
            attemptedCharacterRefSheet,
            characterSeedImageData != null);

        _logger.LogInformation("GeneratePrivateStory completed: storyId={StoryId} title={Title}", storyId, parseResult.Title);
        return new GeneratePrivateStoryResponse
        {
            StoryId = storyId,
            Warnings = warnings
        };
    }

    private static string BuildSystemInstruction(int numberOfPages, string languageCode)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a children's story writer. Reply with plain text only, using exactly these section markers.");
        sb.AppendLine("Format:");
        sb.AppendLine("###T");
        sb.AppendLine("[Story title on one line]");
        sb.AppendLine("###S");
        sb.AppendLine("[Story summary in one short paragraph]");
        for (var i = 1; i <= numberOfPages; i++)
        {
            sb.AppendLine($"###P{i}");
            sb.AppendLine($"[Page {i} text; multiple lines allowed; aim for similar length across pages]");
        }
        sb.AppendLine($"Language: {languageCode}. Generate exactly {numberOfPages} pages. Content suitable for children.");
        sb.AppendLine("Length guidance: keep page lengths roughly similar. Aim for ~220–300 characters per page, with a soft maximum of 300 characters per page (avoid pages that are much longer than others).");
        return sb.ToString();
    }

    private static string BuildUserContent(string idea, string? titleHint, int numberOfPages)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Seed/summary for the story:");
        sb.AppendLine(idea);
        if (!string.IsNullOrWhiteSpace(titleHint))
        {
            sb.AppendLine();
            sb.Append("Suggested title: ").AppendLine(titleHint);
        }
        sb.AppendLine();
        sb.Append($"Generate the complete story in the requested format with exactly {numberOfPages} pages.");
        return sb.ToString();
    }

    private static EditableStoryDto BuildEditableStoryDto(string storyId, string languageCode, StoryTextParseResult parseResult)
    {
        var tiles = parseResult.Pages
            .Select(p => new EditableTileDto
            {
                Type = "page",
                Id = p.PageId,
                Caption = string.Empty,
                Text = p.Text,
                ImageUrl = string.Empty,
                AudioUrl = string.Empty
            })
            .ToList();

        return new EditableStoryDto
        {
            Id = storyId,
            Title = parseResult.Title,
            CoverImageUrl = string.Empty,
            Summary = parseResult.Summary,
            Language = languageCode,
            StoryType = 1,
            Tiles = tiles
        };
    }

    private async Task RetryAsync(Func<Task> action, string operationName, CancellationToken ct)
    {
        await RetryAsync(async () =>
        {
            await action();
            return true;
        }, operationName, ct);
    }

    private async Task<T> RetryAsync<T>(Func<Task<T>> action, string operationName, CancellationToken ct)
    {
        const int maxAttempts = 2;
        var delay = TimeSpan.FromMilliseconds(450);
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex))
            {
                lastException = ex;
                _logger.LogWarning(
                    ex,
                    "Transient failure in {OperationName}. Retrying {Attempt}/{MaxAttempts}...",
                    operationName,
                    attempt + 1,
                    maxAttempts);
                await Task.Delay(delay, ct);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
            catch (Exception ex)
            {
                lastException = ex;
                break;
            }
        }

        throw lastException ?? new InvalidOperationException($"Retry operation '{operationName}' failed.");
    }

    private static bool IsTransient(Exception ex)
    {
        if (ex is GoogleImageGenerationException imgEx)
            return imgEx.IsTransient;

        if (ex is TaskCanceledException or TimeoutException or HttpRequestException)
            return true;

        var message = (ex.Message + " " + ex.InnerException?.Message).ToLowerInvariant();
        return message.Contains("429")
               || message.Contains("too many requests")
               || message.Contains("rate limit")
               || message.Contains("timeout")
               || message.Contains("temporarily unavailable")
               || message.Contains("service unavailable");
    }

    private async Task<string> GenerateStoryTextClassicAsync(
        string idea,
        string? title,
        int pageCount,
        string languageCode,
        string apiKey,
        CancellationToken ct)
    {
        var systemInstruction = BuildSystemInstruction(pageCount, languageCode);
        var userContent = BuildUserContent(idea, title, pageCount);
        return await RetryAsync(
            () => _googleTextService.GenerateContentAsync(
                systemInstruction,
                userContent,
                apiKeyOverride: apiKey,
                modelOverride: null,
                responseMimeType: "text/plain",
                ct: ct),
            "private-story-text",
            ct);
    }

    private async Task<string> GenerateStoryTextWithBibleAsync(
        StoryBible storyBible,
        int pageCount,
        string languageCode,
        string apiKey,
        CancellationToken ct)
    {
        var systemInstruction = StoryBibleTextPromptBuilder.BuildGoogleSystemInstruction(pageCount, languageCode, storyBible);
        var userContent = StoryBibleTextPromptBuilder.BuildGoogleUserContent(storyBible, string.Empty, pageCount);
        return await RetryAsync(
            () => _googleTextService.GenerateContentAsync(
                systemInstruction,
                userContent,
                apiKeyOverride: apiKey,
                modelOverride: null,
                responseMimeType: "text/plain",
                ct: ct),
            "private-story-text-with-bible",
            ct);
    }

    private async Task<(byte[]? ImageData, string? MimeType)> TryGenerateCharacterReferenceSeedAsync(
        StoryBible storyBible,
        string apiKey,
        CancellationToken ct)
    {
        try
        {
            var prompt = BuildCharacterReferenceSheetPrompt(storyBible);
            var (imageData, mimeType) = await RetryAsync(
                () => _googleImageService.GenerateImageFromPromptAsync(
                    prompt,
                    ct,
                    apiKeyOverride: apiKey,
                    modelOverride: null),
                "private-story-character-reference-seed",
                ct);

            return (imageData, mimeType);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Private story character reference seed generation failed. Continuing without seed image.");
            return (null, null);
        }
    }

    private static string BuildCharacterReferenceSheetPrompt(StoryBible storyBible)
    {
        var roster = string.Join(
            Environment.NewLine,
            storyBible.Characters.Select(c =>
            {
                var marker = c.Visual.Accessories.Count > 0 ? string.Join(", ", c.Visual.Accessories) : "no marker";
                var features = c.Visual.Features.Count > 0 ? string.Join(", ", c.Visual.Features) : "distinctive silhouette";
                return $"- {c.Name}: {c.Visual.PrimaryColor} {c.Visual.Size} {c.Species}; features: {features}; unique marker: {marker}";
            }));

        return $@"Character reference sheet for a children's book illustration pipeline.
Portrait 4:5 composition. Soft storybook style. White or very light neutral background.
Show ALL recurring characters in full body, side-by-side, clearly separated, facing camera.
Do not add text, labels, watermark, or speech bubbles.
Keep exact colors, species, proportions, and unique accessories.

CHARACTER ROSTER (IMMUTABLE):
{roster}";
    }

    private void ValidateChildSafetyOrThrow(string idea)
    {
        if (string.IsNullOrWhiteSpace(idea))
            return;

        var match = UnsafePromptPattern.Match(idea);
        if (match.Success)
        {
            _logger.LogWarning(
                "Private story input blocked by child safety gate. matchedTerm={MatchedTerm}",
                match.Value);
            throw new ChildSafetyPolicyViolationException();
        }
    }

    private static List<string> CollectGenerationWarnings(
        EditableStoryDto dto,
        bool generateImages,
        bool generateAudio)
    {
        var warnings = new List<string>();

        if (generateImages)
        {
            var missingImageTiles = dto.Tiles
                .Where(t => string.IsNullOrWhiteSpace(t.ImageUrl))
                .Select(t => t.Id)
                .ToList();

            if (missingImageTiles.Count > 0)
            {
                warnings.Add(
                    $"ImageGenerationPartial: {missingImageTiles.Count} pagini fără imagine ({string.Join(", ", missingImageTiles)}).");
            }
        }

        if (generateAudio)
        {
            var missingAudioTiles = dto.Tiles
                .Where(t => string.IsNullOrWhiteSpace(t.AudioUrl))
                .Select(t => t.Id)
                .ToList();

            if (missingAudioTiles.Count > 0)
            {
                warnings.Add(
                    $"AudioGenerationPartial: {missingAudioTiles.Count} pagini fără audio ({string.Join(", ", missingAudioTiles)}).");
            }
        }

        return warnings;
    }

    private void LogEstimatedStoryCost(
        string storyId,
        EditableStoryDto dto,
        GeneratePrivateStoryRequest request,
        bool attemptedStoryBible,
        bool hasStoryBible,
        bool attemptedScenePlan,
        bool hasScenePlan,
        bool attemptedCharacterRefSheet,
        bool hasCharacterRefSheet)
    {
        var pageTiles = dto.Tiles.Where(t => string.Equals(t.Type, "page", StringComparison.OrdinalIgnoreCase)).ToList();
        var pagesWithText = pageTiles.Count(t => !string.IsNullOrWhiteSpace(t.Text));
        var generatedImages = pageTiles.Count(t => !string.IsNullOrWhiteSpace(t.ImageUrl));
        var generatedAudio = pageTiles.Count(t => !string.IsNullOrWhiteSpace(t.AudioUrl));

        var storyBibleCalls = attemptedStoryBible ? 1 : 0;
        var storyTextCalls = 1;
        var scenePlanCalls = attemptedScenePlan ? 1 : 0;
        var referenceSheetCalls = attemptedCharacterRefSheet ? 1 : 0;
        var imageCalls = request.GenerateImages ? generatedImages : 0;
        var audioCalls = request.GenerateAudio ? generatedAudio : 0;
        var diacriticsCalls = request.GenerateAudio ? pagesWithText : 0;

        var storyBibleCost = storyBibleCalls * StoryBibleCostPerCallUsd;
        var storyTextCost = storyTextCalls * StoryTextCostPerCallUsd;
        var scenePlanCost = scenePlanCalls * ScenePlanCostPerCallUsd;
        var refSheetCost = referenceSheetCalls * CharacterReferenceSheetCostPerCallUsd;
        var imagesCost = imageCalls * StoryImageCostPerCallUsd;
        var audioCost = audioCalls * StoryAudioCostPerCallUsd;
        var diacriticsCost = diacriticsCalls * DiacriticsCostPerCallUsd;
        var totalCost = storyBibleCost + storyTextCost + scenePlanCost + refSheetCost + imagesCost + audioCost + diacriticsCost;

        _logger.LogInformation(
            "Private story estimated cost summary: storyId={StoryId} storyBibleCalls={StoryBibleCalls} storyBibleOk={StoryBibleOk} textCalls={TextCalls} scenePlanCalls={ScenePlanCalls} scenePlanOk={ScenePlanOk} refSheetCalls={RefSheetCalls} refSheetOk={RefSheetOk} imageCalls={ImageCalls} audioCalls={AudioCalls} diacriticsCalls={DiacriticsCalls} estTotalUsd={EstTotalUsd:F3}",
            storyId,
            storyBibleCalls,
            hasStoryBible,
            storyTextCalls,
            scenePlanCalls,
            hasScenePlan,
            referenceSheetCalls,
            hasCharacterRefSheet,
            imageCalls,
            audioCalls,
            diacriticsCalls,
            totalCost);
    }
}
