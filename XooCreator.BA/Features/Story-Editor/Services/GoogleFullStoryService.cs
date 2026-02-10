using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for generating a complete story (text + images) using Google Gemini services.
/// Aggregates GoogleTextService and GoogleImageService for efficient full story generation.
/// </summary>
public interface IGoogleFullStoryService
{
    /// <summary>
    /// Generates a complete story with the specified number of pages.
    /// </summary>
    /// <param name="title">Story title.</param>
    /// <param name="summary">Story summary/description.</param>
    /// <param name="languageCode">Language code (e.g., "ro-RO", "en-US").</param>
    /// <param name="ageGroupIds">Age group IDs for targeting.</param>
    /// <param name="topicIds">Topic IDs for story themes.</param>
    /// <param name="numberOfPages">Number of pages to generate (1-10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Generated story pages with text and images. Each page contains:
    /// - Text content
    /// - Image data (bytes) and MIME type
    /// </returns>
    Task<List<GeneratedStoryPage>> GenerateFullStoryAsync(
        string title,
        string summary,
        string languageCode,
        List<string>? ageGroupIds = null,
        List<string>? topicIds = null,
        int numberOfPages = 5,
        string? storyInstructions = null,
        bool generateImages = false,
        bool generateAudio = false,
        CancellationToken ct = default);
}

/// <summary>
/// Represents a generated story page with text, image, and audio.
/// </summary>
public class GeneratedStoryPage
{
    public int PageNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public byte[]? ImageData { get; set; }
    public string ImageMimeType { get; set; } = "image/png";
    public byte[]? AudioData { get; set; }
    public string AudioFormat { get; set; } = "wav";
}

/// <summary>
/// Service that generates complete stories by aggregating text and image generation services.
/// Optimized for cost efficiency by using Flash models and minimal context.
/// </summary>
public class GoogleFullStoryService : IGoogleFullStoryService
{
    private readonly IGoogleTextService _textService;
    private readonly IGoogleImageService _imageService;
    private readonly IGoogleAudioGeneratorService _audioService;
    private readonly ILogger<GoogleFullStoryService> _logger;

    public GoogleFullStoryService(
        IGoogleTextService textService,
        IGoogleImageService imageService,
        IGoogleAudioGeneratorService audioService,
        ILogger<GoogleFullStoryService> logger)
    {
        _textService = textService;
        _imageService = imageService;
        _audioService = audioService;
        _logger = logger;
    }

    public async Task<List<GeneratedStoryPage>> GenerateFullStoryAsync(
        string title,
        string summary,
        string languageCode,
        List<string>? ageGroupIds = null,
        List<string>? topicIds = null,
        int numberOfPages = 5,
        string? storyInstructions = null,
        bool generateImages = false,
        bool generateAudio = false,
        CancellationToken ct = default)
    {
        if (numberOfPages < 1 || numberOfPages > 10)
            throw new ArgumentException("Number of pages must be between 1 and 10", nameof(numberOfPages));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary cannot be empty", nameof(summary));

        _logger.LogInformation(
            "Starting full story generation. Title: {Title}, Pages: {Pages}, Language: {Language}",
            title, numberOfPages, languageCode);

        var pages = new List<GeneratedStoryPage>();
        var storyContext = new StringBuilder();

        // Build initial context from metadata (optimized - no full JSON)
        var contextPrompt = BuildInitialContext(title, summary, ageGroupIds, topicIds, languageCode);

        for (int pageNum = 1; pageNum <= numberOfPages; pageNum++)
        {
            _logger.LogInformation("Generating page {PageNumber} of {TotalPages}", pageNum, numberOfPages);

            // Generate text for this page
            // We build a minimal story JSON with only previous pages for context
            var storyJsonForPage = BuildStoryJsonForPage(
                title,
                summary,
                pages,
                pageNum,
                ageGroupIds,
                topicIds);

            // Combine extra instructions with story instructions
            var extraInstructions = BuildExtraInstructions(ageGroupIds, topicIds, storyInstructions);

            var pageText = await _textService.GenerateNextPageAsync(
                storyJsonForPage,
                languageCode,
                extraInstructions,
                ct);

            // Extract caption (first sentence or first 50 chars)
            var caption = ExtractCaption(pageText);

            // Generate image for this page (if requested)
            byte[]? imageData = null;
            string imageMimeType = "image/png";

            if (generateImages)
            {
                try
                {
                    // Use previous page's image as reference for consistency (if available)
                    byte[]? referenceImage = pages.Count > 0 && pages[^1].ImageData != null
                        ? pages[^1].ImageData
                        : null;

                    var (imgData, mimeType) = await _imageService.GenerateStoryImageAsync(
                        storyJsonForPage,
                        pageText,
                        languageCode,
                        extraInstructions,
                        referenceImage,
                        referenceImageMimeType: pages.Count > 0 ? pages[^1].ImageMimeType : null,
                        ct);

                    imageData = imgData;
                    imageMimeType = mimeType;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate image for page {PageNumber}, continuing without image", pageNum);
                    // Continue without image - text is more important
                }
            }

            // Generate audio for this page (if requested)
            byte[]? audioData = null;
            string audioFormat = "wav";

            if (generateAudio)
            {
                try
                {
                    var (audData, format) = await _audioService.GenerateAudioAsync(
                        pageText,
                        languageCode,
                        null, // Use default voice
                        null, // Use default style instructions from config
                        apiKeyOverride: null, // Use default from config
                        ttsModelOverride: null,
                        ct);

                    audioData = audData;
                    audioFormat = format;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate audio for page {PageNumber}, continuing without audio", pageNum);
                    // Continue without audio
                }
            }

            var page = new GeneratedStoryPage
            {
                PageNumber = pageNum,
                Text = pageText,
                Caption = caption,
                ImageData = imageData,
                ImageMimeType = imageMimeType,
                AudioData = audioData,
                AudioFormat = audioFormat
            };

            pages.Add(page);

            // Update context for next iteration (only last 2 pages for efficiency)
            storyContext.Clear();
            foreach (var p in pages.TakeLast(2))
            {
                storyContext.AppendLine($"Page {p.PageNumber}: {p.Text}");
            }
        }

        _logger.LogInformation("Successfully generated {PageCount} pages", pages.Count);
        return pages;
    }

    /// <summary>
    /// Builds a minimal story JSON with only essential context for the next page generation.
    /// </summary>
    private static string BuildStoryJsonForPage(
        string title,
        string summary,
        List<GeneratedStoryPage> existingPages,
        int currentPageNumber,
        List<string>? ageGroupIds,
        List<string>? topicIds)
    {
        // Build minimal JSON with only last 1 page for context (cost optimization - 1 page is enough for continuity)
        var lastPages = existingPages.TakeLast(1).ToList();

        var storyData = new Dictionary<string, object?>
        {
            ["storyId"] = "generating",
            ["title"] = title,
            ["summary"] = summary,
            ["languageCode"] = "temp"
        };

        if (ageGroupIds != null && ageGroupIds.Count > 0)
            storyData["ageGroupIds"] = ageGroupIds;

        if (topicIds != null && topicIds.Count > 0)
            storyData["topicIds"] = topicIds;

        var tiles = new List<object>();
        foreach (var page in lastPages)
        {
            tiles.Add(new Dictionary<string, object?>
            {
                ["tileId"] = $"p{page.PageNumber}",
                ["type"] = "page",
                ["sortOrder"] = page.PageNumber,
                ["caption"] = page.Caption,
                ["text"] = page.Text
            });
        }

        storyData["tiles"] = tiles;

        return JsonSerializer.Serialize(storyData, new JsonSerializerOptions
        {
            WriteIndented = false // Compact for token efficiency
        });
    }

    /// <summary>
    /// Builds initial context prompt from metadata.
    /// </summary>
    private static string BuildInitialContext(
        string title,
        string summary,
        List<string>? ageGroupIds,
        List<string>? topicIds,
        string languageCode)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Title: {title}");
        sb.AppendLine($"Summary: {summary}");
        sb.AppendLine($"Language: {languageCode}");

        if (ageGroupIds != null && ageGroupIds.Count > 0)
            sb.AppendLine($"Age Groups: {string.Join(", ", ageGroupIds)}");

        if (topicIds != null && topicIds.Count > 0)
            sb.AppendLine($"Topics: {string.Join(", ", topicIds)}");

        return sb.ToString();
    }

    /// <summary>
    /// Builds extra instructions based on age groups, topics, and story instructions.
    /// </summary>
    private static string? BuildExtraInstructions(List<string>? ageGroupIds, List<string>? topicIds, string? storyInstructions)
    {
        var instructions = new List<string>();

        if (ageGroupIds != null && ageGroupIds.Count > 0)
        {
            instructions.Add($"Target age groups: {string.Join(", ", ageGroupIds)}");
        }

        if (topicIds != null && topicIds.Count > 0)
        {
            instructions.Add($"Story themes: {string.Join(", ", topicIds)}");
        }

        if (!string.IsNullOrWhiteSpace(storyInstructions))
        {
            instructions.Add($"Story building instructions: {storyInstructions}");
        }

        return instructions.Count > 0 ? string.Join(". ", instructions) : null;
    }

    /// <summary>
    /// Extracts a caption from page text (first sentence or first 50 characters).
    /// </summary>
    private static string ExtractCaption(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Try to get first sentence
        var firstSentenceEnd = text.IndexOfAny(new[] { '.', '!', '?' });
        if (firstSentenceEnd > 0 && firstSentenceEnd <= 100)
        {
            return text.Substring(0, firstSentenceEnd + 1).Trim();
        }

        // Fallback to first 50 characters
        return text.Length > 50 ? text.Substring(0, 50).Trim() + "..." : text.Trim();
    }
}

