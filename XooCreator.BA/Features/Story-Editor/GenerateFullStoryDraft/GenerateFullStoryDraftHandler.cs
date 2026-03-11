using System.Text;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Creates draft, generates full story text with user API key, parses ###T/###S/###P,
/// optionally generates images/audio via IGenerateFullStoryDraftAssetsGenerator, builds EditableStoryDto, saves draft.
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
    private readonly ILogger<GenerateFullStoryDraftHandler> _logger;

    public GenerateFullStoryDraftHandler(
        IStoryIdGenerator storyIdGenerator,
        IStoryEditorService editorService,
        IGoogleTextService googleTextService,
        IOpenAITextWithApiKey openAITextWithApiKey,
        IGenerateFullStoryDraftAssetsGenerator assetsGenerator,
        ILogger<GenerateFullStoryDraftHandler> logger)
    {
        _storyIdGenerator = storyIdGenerator;
        _editorService = editorService;
        _googleTextService = googleTextService;
        _openAITextWithApiKey = openAITextWithApiKey;
        _assetsGenerator = assetsGenerator;
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

        var storyId = await _storyIdGenerator.GenerateNextAsync(ownerUserId, ownerFirstName, ownerLastName, ct);
        _logger.LogInformation("GenerateFullStoryDraft: storyId={StoryId} provider={Provider} pages={Pages}",
            storyId, request.Provider, request.NumberOfPages);

        await _editorService.EnsureDraftAsync(ownerUserId, storyId, StoryType.Indie, ct);
        await _editorService.EnsureTranslationAsync(ownerUserId, storyId, request.LanguageCode, "AI Story", ct);

        var rawText = await GenerateFullStoryTextAsync(request, ct);
        var parseResult = StoryTextDelimiterParser.Parse(rawText);

        var dto = BuildEditableStoryDto(storyId, request.LanguageCode, parseResult);

        if (request.GenerateImages || request.GenerateAudio)
        {
            var isOpenAi = string.Equals(request.Provider.Trim(), "OpenAI", StringComparison.OrdinalIgnoreCase);
            await _assetsGenerator.FillImagesAndAudioAsync(request, dto, storyId, ownerEmail ?? string.Empty, isOpenAi, usePublishedPaths: false, ct);
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
