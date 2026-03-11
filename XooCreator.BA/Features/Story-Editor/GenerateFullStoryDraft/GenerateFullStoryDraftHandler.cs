using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Creates draft, generates full story text with user API key, parses ###T/###S/###P,
/// optionally generates images/audio via IGenerateFullStoryDraftAssetsGenerator, builds EditableStoryDto, saves draft.
/// Supports Story Bible pipeline for improved character consistency when UseStoryBible is enabled.
/// </summary>
public sealed class GenerateFullStoryDraftHandler : IGenerateFullStoryDraftHandler
{
    private const int MinTextSeedLength = 20;
    private const int MaxInstructionsLength = 3000;

    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly IStoryEditorService _editorService;
    private readonly IGoogleTextService _googleTextService;
    private readonly IOpenAITextWithApiKey _openAITextWithApiKey;
    private readonly IGenerateFullStoryDraftAssetsGenerator _assetsGenerator;
    private readonly IStoryBibleGenerator _storyBibleGenerator;
    private readonly IStoryValidator _storyValidator;
    private readonly IStoryRepairService _storyRepairService;
    private readonly IScenePlanner _scenePlanner;
    private readonly IIllustrationPromptBuilder _illustrationPromptBuilder;
    private readonly ILogger<GenerateFullStoryDraftHandler> _logger;

    public GenerateFullStoryDraftHandler(
        IStoryIdGenerator storyIdGenerator,
        IStoryEditorService editorService,
        IGoogleTextService googleTextService,
        IOpenAITextWithApiKey openAITextWithApiKey,
        IGenerateFullStoryDraftAssetsGenerator assetsGenerator,
        IStoryBibleGenerator storyBibleGenerator,
        IStoryValidator storyValidator,
        IStoryRepairService storyRepairService,
        IScenePlanner scenePlanner,
        IIllustrationPromptBuilder illustrationPromptBuilder,
        ILogger<GenerateFullStoryDraftHandler> logger)
    {
        _storyIdGenerator = storyIdGenerator;
        _editorService = editorService;
        _googleTextService = googleTextService;
        _openAITextWithApiKey = openAITextWithApiKey;
        _assetsGenerator = assetsGenerator;
        _storyBibleGenerator = storyBibleGenerator;
        _storyValidator = storyValidator;
        _storyRepairService = storyRepairService;
        _scenePlanner = scenePlanner;
        _illustrationPromptBuilder = illustrationPromptBuilder;
        _logger = logger;
    }

    public async Task<GenerateFullStoryDraftResponse> ExecuteAsync(
        GenerateFullStoryDraftRequest request,
        Guid ownerUserId,
        string ownerFirstName,
        string ownerLastName,
        string ownerEmail,
        CancellationToken ct = default)
    {
        ValidateRequest(request);

        var provider = (request.Provider ?? "Google").Trim();
        var isOpenAiProvider = string.Equals(provider, "OpenAI", StringComparison.OrdinalIgnoreCase);
        var useStoryBiblePipeline = request.UseStoryBible && !isOpenAiProvider;

        var storyId = await _storyIdGenerator.GenerateNextAsync(ownerUserId, ownerFirstName, ownerLastName, ct);
        _logger.LogInformation("GenerateFullStoryDraft: storyId={StoryId} provider={Provider} pages={Pages} useStoryBible={UseStoryBible}",
            storyId, request.Provider, request.NumberOfPages, request.UseStoryBible);

        await _editorService.EnsureDraftAsync(ownerUserId, storyId, StoryType.Indie, ct);
        await _editorService.EnsureTranslationAsync(ownerUserId, storyId, request.LanguageCode, "AI Story", ct);

        StoryBible? storyBible = null;
        ScenePlan? scenePlan = null;

        // Story Bible Pipeline (when enabled)
        if (useStoryBiblePipeline)
        {
            _logger.LogInformation("Using Story Bible pipeline for improved consistency (Google-only)");

            try
            {
                // Step 1: Generate Story Bible
                storyBible = await _storyBibleGenerator.GenerateAsync(
                    request.TextSeed.Trim(),
                    request.Title,
                    request.NumberOfPages,
                    request.LanguageCode,
                    request.ApiKey.Trim(),
                    request.TextModel,
                    ct);
                _logger.LogInformation("Story Bible pipeline: StoryBibleUsed=true StoryId={StoryId}", storyId);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                useStoryBiblePipeline = false;
                storyBible = null;
                _logger.LogWarning(ex,
                    "Story Bible generation failed for storyId={StoryId}. Falling back to classic flow.",
                    storyId);
            }
        }
        else if (request.UseStoryBible && isOpenAiProvider)
        {
            _logger.LogInformation(
                "Story Bible pipeline disabled for storyId={StoryId} because provider=OpenAI. Falling back to classic flow.",
                storyId);
        }

        // Generate story text (with or without Bible)
        var rawText = useStoryBiblePipeline && storyBible != null
            ? await GenerateStoryTextWithBibleAsync(request, storyBible, ct)
            : await GenerateFullStoryTextAsync(request, ct);

        var parseResult = StoryTextDelimiterParser.Parse(rawText);

        // Validate and repair (when Story Bible is enabled)
        if (useStoryBiblePipeline && storyBible != null)
        {
            StoryValidationResult? validation = null;

            try
            {
                validation = await _storyValidator.ValidateAsync(
                    storyBible,
                    rawText,
                    request.ApiKey.Trim(),
                    request.TextModel,
                    ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogWarning(ex,
                    "Story Bible validation failed for storyId={StoryId}. Continuing without validator.",
                    storyId);
            }

            // Guardrail: only run Repair when we have error/critical issues (IsValid=false)
            if (validation is { IsValid: false } && validation.Issues.Count > 0)
            {
                try
                {
                    _logger.LogInformation("Story validation found {IssueCount} issues, attempting repair", validation.Issues.Count);
                    rawText = await _storyRepairService.RepairAsync(
                        storyBible,
                        rawText,
                        validation,
                        request.ApiKey.Trim(),
                        request.TextModel,
                        ct);
                    parseResult = StoryTextDelimiterParser.Parse(rawText);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogWarning(ex,
                        "Story Bible repair failed for storyId={StoryId}. Keeping original story text.",
                        storyId);
                }
            }

            // Generate scene plan for images (best-effort)
            if (request.GenerateImages)
            {
                try
                {
                    var pageTexts = parseResult.Pages.Select(p => p.Text).ToList();
                    scenePlan = await _scenePlanner.PlanAsync(
                        storyBible,
                        pageTexts,
                        storyId,
                        request.ApiKey.Trim(),
                        request.TextModel,
                        ct);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    scenePlan = null;
                    _logger.LogWarning(ex,
                        "Scene planning failed for storyId={StoryId}. Falling back to classic image prompts.",
                        storyId);
                }
            }
        }

        var dto = BuildEditableStoryDto(storyId, request.LanguageCode, parseResult);

        if (request.GenerateImages || request.GenerateAudio)
        {
            var isOpenAi = isOpenAiProvider;

            // Pass Story Bible context to asset generator
            await _assetsGenerator.FillImagesAndAudioAsync(
                request, 
                dto, 
                storyId, 
                ownerEmail ?? string.Empty, 
                isOpenAi, 
                usePublishedPaths: false, 
                ct,
                storyBible,
                scenePlan,
                _illustrationPromptBuilder);
        }

        await _editorService.SaveDraftAsync(ownerUserId, storyId, request.LanguageCode, dto, false, null, ct);

        _logger.LogInformation("GenerateFullStoryDraft completed: storyId={StoryId} title={Title} pages={Pages}",
            storyId, parseResult.Title, parseResult.Pages.Count);

        return new GenerateFullStoryDraftResponse { StoryId = storyId };
    }

    private static void ValidateRequest(GenerateFullStoryDraftRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ApiKey))
            throw new ArgumentException("API key is required.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.TextSeed) || request.TextSeed.Trim().Length < MinTextSeedLength)
            throw new ArgumentException($"Text seed must be at least {MinTextSeedLength} characters.", nameof(request));
        if (request.NumberOfPages < 1 || request.NumberOfPages > 10)
            throw new ArgumentException("Number of pages must be between 1 and 10.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.LanguageCode))
            throw new ArgumentException("Language code is required.", nameof(request));
        var provider = (request.Provider ?? "Google").Trim();
        if (!string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Provider must be Google or OpenAI.", nameof(request));
        if (request.Instructions != null && request.Instructions.Length > MaxInstructionsLength)
            throw new ArgumentException($"Instructions must be at most {MaxInstructionsLength} characters.", nameof(request));
    }

    private async Task<string> GenerateFullStoryTextAsync(GenerateFullStoryDraftRequest request, CancellationToken ct)
    {
        var isOpenAi = string.Equals(request.Provider.Trim(), "OpenAI", StringComparison.OrdinalIgnoreCase);
        var n = request.NumberOfPages;
        var lang = request.LanguageCode;
        var seed = request.TextSeed.Trim();
        var instructions = request.Instructions?.Trim() ?? string.Empty;
        var titleHint = request.Title?.Trim() ?? string.Empty;

        if (isOpenAi)
        {
            var title = string.IsNullOrWhiteSpace(titleHint) ? "Story" : titleHint;
            return await _openAITextWithApiKey.GenerateFullStoryTextAsync(
                title,
                seed,
                lang,
                n,
                null,
                null,
                instructions,
                request.ApiKey.Trim(),
                request.TextModel,
                ct);
        }

        var systemInstruction = BuildGoogleSystemInstruction(n, lang);
        var userContent = BuildGoogleUserContent(seed, instructions, titleHint, n);
        return await _googleTextService.GenerateContentAsync(
            systemInstruction,
            userContent,
            request.ApiKey.Trim(),
            request.TextModel,
            responseMimeType: "text/plain",
            ct);
    }

    /// <summary>
    /// Generate story text using Story Bible for improved consistency.
    /// </summary>
    private async Task<string> GenerateStoryTextWithBibleAsync(
        GenerateFullStoryDraftRequest request,
        StoryBible storyBible,
        CancellationToken ct)
    {
        var n = request.NumberOfPages;
        var lang = request.LanguageCode;
        var instructions = request.Instructions?.Trim() ?? string.Empty;

        var systemInstruction = BuildGoogleSystemInstructionWithBible(n, lang, storyBible);
        var userContent = BuildGoogleUserContentWithBible(storyBible, instructions, n);

        return await _googleTextService.GenerateContentAsync(
            systemInstruction,
            userContent,
            request.ApiKey.Trim(),
            request.TextModel,
            responseMimeType: "text/plain",
            ct);
    }

    private static string BuildGoogleSystemInstructionWithBible(int numberOfPages, string languageCode, StoryBible bible)
    {
        var characterList = string.Join("\n", bible.Characters.Select(c =>
            $"- {c.Name}: {c.Visual.PrimaryColor} {c.Species}, {c.Visual.Size}. Features: {string.Join(", ", c.Visual.Features)}. Personality: {string.Join(", ", c.Personality)}"));

        var sb = new StringBuilder();
        sb.AppendLine("You are a children's story writer. Write a story based on the Story Bible below.");
        sb.AppendLine();
        sb.AppendLine("STORY BIBLE:");
        sb.AppendLine($"Title: {bible.Title}");
        sb.AppendLine($"Tone: {bible.Tone}");
        sb.AppendLine($"Setting: {bible.Setting.Place}, {bible.Setting.Time}");
        sb.AppendLine($"Plot: {bible.Plot.Problem} → {bible.Plot.Resolution}. Moral: {bible.Plot.Moral}");
        sb.AppendLine();
        sb.AppendLine("CHARACTERS (mention their colors/features naturally in the story):");
        sb.AppendLine(characterList);
        sb.AppendLine();
        sb.AppendLine("SCENE SKELETON:");
        for (var i = 0; i < bible.SceneSkeleton.Count && i < numberOfPages; i++)
        {
            sb.AppendLine($"- Page {i + 1}: {bible.SceneSkeleton[i]}");
        }
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT (strict):");
        sb.AppendLine("###T");
        sb.AppendLine("[Story title on one line]");
        sb.AppendLine("###S");
        sb.AppendLine("[Story summary in one short paragraph]");
        for (var i = 1; i <= numberOfPages; i++)
        {
            sb.AppendLine($"###P{i}");
            sb.AppendLine($"[Page {i} text - follow the scene skeleton]");
        }
        sb.AppendLine($"Language: {languageCode}. Generate exactly {numberOfPages} pages.");
        sb.AppendLine();
        sb.AppendLine("IMPORTANT: Naturally mention character visual details (colors, features) throughout the story.");
        return sb.ToString();
    }

    private static string BuildGoogleUserContentWithBible(StoryBible bible, string instructions, int numberOfPages)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Write the story '{bible.Title}' following the Story Bible.");
        if (!string.IsNullOrWhiteSpace(instructions))
        {
            sb.AppendLine();
            sb.AppendLine("Additional instructions:");
            sb.AppendLine(instructions);
        }
        sb.AppendLine();
        sb.AppendLine($"Generate exactly {numberOfPages} pages, one for each scene in the skeleton.");
        sb.AppendLine("Make sure to mention character visual details naturally in the narrative.");
        return sb.ToString();
    }

    private static string BuildGoogleSystemInstruction(int numberOfPages, string languageCode)
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
            sb.AppendLine($"[Page {i} text; multiple lines allowed]");
        }
        sb.AppendLine($"Language: {languageCode}. Generate exactly {numberOfPages} pages.");
        return sb.ToString();
    }

    private static string BuildGoogleUserContent(string textSeed, string instructions, string titleHint, int numberOfPages)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Seed/summary for the story:");
        sb.AppendLine(textSeed);
        if (!string.IsNullOrWhiteSpace(instructions))
        {
            sb.AppendLine();
            sb.AppendLine("Additional instructions:");
            sb.AppendLine(instructions);
        }
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
}
