using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
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

    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly IGoogleTextService _googleTextService;
    private readonly IGenerateFullStoryDraftAssetsGenerator _assetsGenerator;
    private readonly IPrivateStoryCreationService _privateStoryCreation;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeneratePrivateStoryHandler> _logger;

    public GeneratePrivateStoryHandler(
        IStoryIdGenerator storyIdGenerator,
        IGoogleTextService googleTextService,
        IGenerateFullStoryDraftAssetsGenerator assetsGenerator,
        IPrivateStoryCreationService privateStoryCreation,
        IConfiguration configuration,
        ILogger<GeneratePrivateStoryHandler> logger)
    {
        _storyIdGenerator = storyIdGenerator;
        _googleTextService = googleTextService;
        _assetsGenerator = assetsGenerator;
        _privateStoryCreation = privateStoryCreation;
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

        var systemInstruction = BuildSystemInstruction(pageCount, lang);
        var userContent = BuildUserContent(request.Idea.Trim(), request.Title?.Trim(), pageCount);
        var apiKey = _configuration["GoogleAI:ApiKey"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("GoogleAI:ApiKey is not configured for private story generation.");

        var rawText = await RetryAsync(
            () => _googleTextService.GenerateContentAsync(
                systemInstruction,
                userContent,
                apiKeyOverride: apiKey,
                modelOverride: null,
                responseMimeType: "text/plain",
                ct: ct),
            "private-story-text",
            ct);

        var parseResult = StoryTextDelimiterParser.Parse(rawText);
        var dto = BuildEditableStoryDto(storyId, lang, parseResult);

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
            AudioModel = "gemini-2.5-pro-tts",
            Title = request.Title
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
                    ct),
                "private-story-assets",
                ct);
        }

        await _privateStoryCreation.CreateFromDtoAsync(dto, storyId, ownerUserId, ownerEmail ?? string.Empty, lang, ct);

        _logger.LogInformation("GeneratePrivateStory completed: storyId={StoryId} title={Title}", storyId, parseResult.Title);
        return new GeneratePrivateStoryResponse { StoryId = storyId };
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
}
