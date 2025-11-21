using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for generating a complete story (text + images) using OpenAI services.
/// Aggregates OpenAITextService and OpenAIImageService for efficient full story generation.
/// </summary>
public interface IOpenAIFullStoryService
{
    /// <summary>
    /// Generates a complete story with the specified number of pages.
    /// </summary>
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
/// Service that generates complete stories by aggregating OpenAI text and image generation services.
/// </summary>
public class OpenAIFullStoryService : IOpenAIFullStoryService
{
    private readonly IOpenAITextService _textService;
    private readonly IOpenAIImageService _imageService;
    private readonly IOpenAIAudioGeneratorService _audioService;
    private readonly ILogger<OpenAIFullStoryService> _logger;

    public OpenAIFullStoryService(
        IOpenAITextService textService,
        IOpenAIImageService imageService,
        IOpenAIAudioGeneratorService audioService,
        ILogger<OpenAIFullStoryService> logger)
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
            "Starting OpenAI full story generation. Title: {Title}, Pages: {Pages}, Language: {Language}",
            title, numberOfPages, languageCode);

        // Generate complete story text in one request (Opțiunea B)
        _logger.LogInformation("Generating complete story text in single request");
        var fullStoryText = await _textService.GenerateFullStoryTextAsync(
            title,
            summary,
            languageCode,
            numberOfPages,
            ageGroupIds,
            topicIds,
            storyInstructions,
            ct);

        // Parse the full story text into individual pages
        var parsedPages = ParseFullStoryIntoPages(fullStoryText, numberOfPages);
        
        if (parsedPages.Count != numberOfPages)
        {
            _logger.LogWarning(
                "Expected {ExpectedPages} pages but parsed {ActualPages} pages. Attempting to split evenly.",
                numberOfPages, parsedPages.Count);
            
            // Fallback: split the text evenly if parsing failed
            parsedPages = SplitTextEvenlyIntoPages(fullStoryText, numberOfPages);
        }

        var pages = new List<GeneratedStoryPage>();

        // Generate images and audio for each parsed page
        for (int pageNum = 0; pageNum < parsedPages.Count; pageNum++)
        {
            var pageText = parsedPages[pageNum];
            var pageNumber = pageNum + 1;
            
            _logger.LogInformation("Processing page {PageNumber} of {TotalPages}", pageNumber, numberOfPages);

            var caption = ExtractCaption(pageText);

            byte[]? imageData = null;
            string imageMimeType = "image/png";

            if (generateImages)
            {
                try
                {
                    // Build minimal story JSON for image generation context
                    var storyJsonForImage = BuildStoryJsonForImage(
                        title,
                        summary,
                        pages,
                        pageNumber,
                        pageText,
                        ageGroupIds,
                        topicIds);

                    var extraInstructions = BuildExtraInstructions(ageGroupIds, topicIds, storyInstructions);

                    byte[]? referenceImage = pages.Count > 0 && pages[^1].ImageData != null
                        ? pages[^1].ImageData
                        : null;

                    var (imgData, mimeType) = await _imageService.GenerateStoryImageAsync(
                        storyJsonForImage,
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
                }
            }

            byte[]? audioData = null;
            string audioFormat = "mp3";

            if (generateAudio)
            {
                try
                {
                    var (audData, format) = await _audioService.GenerateAudioAsync(
                        pageText,
                        languageCode,
                        null,
                        ct);

                    audioData = audData;
                    audioFormat = format;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate audio for page {PageNumber}, continuing without audio", pageNum);
                }
            }

            var page = new GeneratedStoryPage
            {
                PageNumber = pageNumber,
                Text = pageText,
                Caption = caption,
                ImageData = imageData,
                ImageMimeType = imageMimeType,
                AudioData = audioData,
                AudioFormat = audioFormat
            };

            pages.Add(page);
        }

        _logger.LogInformation("Successfully generated {PageCount} pages with OpenAI", pages.Count);
        return pages;
    }

    private static string BuildStoryJsonForPage(
        string title,
        string summary,
        List<GeneratedStoryPage> existingPages,
        int currentPageNumber,
        List<string>? ageGroupIds,
        List<string>? topicIds)
    {
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
            WriteIndented = false
        });
    }

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

    private static string? BuildExtraInstructions(List<string>? ageGroupIds, List<string>? topicIds, string? storyInstructions)
    {
        var instructions = new List<string>();

        if (ageGroupIds != null && ageGroupIds.Count > 0)
        {
            instructions.Add($"Target age groups: {string.Join(", ", ageGroupIds)}");
        }

        if (topicIds != null && topicIds.Count > 0)
        {
            instructions.Add($"Topics: {string.Join(", ", topicIds)}");
        }

        if (!string.IsNullOrWhiteSpace(storyInstructions))
        {
            instructions.Add(storyInstructions);
        }

        return instructions.Count > 0 ? string.Join(". ", instructions) : null;
    }

    private static string ExtractCaption(string pageText)
    {
        if (string.IsNullOrWhiteSpace(pageText))
            return string.Empty;

        var sentences = pageText.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        if (sentences.Length > 0)
        {
            var firstSentence = sentences[0].Trim();
            return firstSentence.Length > 50 ? firstSentence[..50] + "..." : firstSentence;
        }

        return pageText.Length > 50 ? pageText[..50] + "..." : pageText;
    }

    /// <summary>
    /// Parses the full story text into individual pages by detecting "Page X:" delimiters.
    /// </summary>
    private static List<string> ParseFullStoryIntoPages(string fullStoryText, int expectedPages)
    {
        var pages = new List<string>();
        
        // Try to find "Page X:" or "Page X" patterns (case insensitive, with optional whitespace)
        var pagePattern = new Regex(
            @"Page\s+(\d+)\s*:",
            RegexOptions.IgnoreCase);
        
        var matches = pagePattern.Matches(fullStoryText);
        
        if (matches.Count == 0)
        {
            // No page delimiters found, return empty list (will trigger fallback)
            return pages;
        }

        // Extract pages based on delimiters
        for (int i = 0; i < matches.Count; i++)
        {
            var startIndex = matches[i].Index + matches[i].Length;
            var endIndex = i < matches.Count - 1 
                ? matches[i + 1].Index 
                : fullStoryText.Length;
            
            var pageText = fullStoryText.Substring(startIndex, endIndex - startIndex).Trim();
            
            if (!string.IsNullOrWhiteSpace(pageText))
            {
                pages.Add(pageText);
            }
        }

        return pages;
    }

    /// <summary>
    /// Fallback: splits text evenly into pages if parsing by delimiters failed.
    /// </summary>
    private static List<string> SplitTextEvenlyIntoPages(string fullStoryText, int numberOfPages)
    {
        var pages = new List<string>();
        var textLength = fullStoryText.Length;
        var pageLength = textLength / numberOfPages;

        for (int i = 0; i < numberOfPages; i++)
        {
            var startIndex = i * pageLength;
            var length = i == numberOfPages - 1 
                ? textLength - startIndex 
                : pageLength;
            
            var pageText = fullStoryText.Substring(startIndex, length).Trim();
            
            // Try to break at sentence boundaries
            if (i < numberOfPages - 1)
            {
                var lastSentenceEnd = Math.Max(
                    pageText.LastIndexOf('.'),
                    Math.Max(
                        pageText.LastIndexOf('!'),
                        pageText.LastIndexOf('?')));
                
                if (lastSentenceEnd > pageLength * 0.7) // If we found a sentence end in the last 30% of the page
                {
                    pageText = pageText.Substring(0, lastSentenceEnd + 1).Trim();
                    startIndex += lastSentenceEnd + 1;
                }
            }
            
            pages.Add(pageText);
        }

        return pages;
    }

    /// <summary>
    /// Builds minimal story JSON for image generation context.
    /// </summary>
    private static string BuildStoryJsonForImage(
        string title,
        string summary,
        List<GeneratedStoryPage> existingPages,
        int currentPageNumber,
        string currentPageText,
        List<string>? ageGroupIds,
        List<string>? topicIds)
    {
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

        // Add current page
        tiles.Add(new Dictionary<string, object?>
        {
            ["tileId"] = $"p{currentPageNumber}",
            ["type"] = "page",
            ["sortOrder"] = currentPageNumber,
            ["text"] = currentPageText
        });

        storyData["tiles"] = tiles;

        return JsonSerializer.Serialize(storyData, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }
}

