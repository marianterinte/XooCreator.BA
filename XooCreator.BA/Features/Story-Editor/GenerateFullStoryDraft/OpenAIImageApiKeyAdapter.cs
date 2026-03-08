using System.Text;
using System.Text.Json;
using OpenAI.Images;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Adapter that calls OpenAI image API with a caller-provided API key.
/// Does not modify existing OpenAIImageService; used only by Generate Full Story Draft.
/// Prompt must be at most 1500 characters (validation, no truncation).
/// </summary>
public class OpenAIImageApiKeyAdapter : IOpenAIImageWithApiKey
{
    public const int MaxImagePromptLength = 1500;
    private readonly string _model;
    private readonly string _sizeConfig;
    private readonly ILogger<OpenAIImageApiKeyAdapter> _logger;

    public OpenAIImageApiKeyAdapter(
        IConfiguration configuration,
        ILogger<OpenAIImageApiKeyAdapter> logger)
    {
        _model = configuration["OpenAI:Image:Model"] ?? "gpt-image-1";
        _sizeConfig = configuration["OpenAI:Image:Size"] ?? "1024x1024";
        _logger = logger;
    }

    public async Task<(byte[] ImageData, string MimeType)> GenerateStoryImageAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions = null,
        byte[]? referenceImage = null,
        string? referenceImageMimeType = null,
        string? apiKeyOverride = null,
        string? modelOverride = null,
        string? imageQualityOverride = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storyJson))
            throw new ArgumentException("Story JSON cannot be empty", nameof(storyJson));
        if (string.IsNullOrWhiteSpace(tileText))
            throw new ArgumentException("Tile text cannot be empty", nameof(tileText));
        if (string.IsNullOrWhiteSpace(apiKeyOverride))
            throw new ArgumentException("API key is required for Generate Full Story Draft.", nameof(apiKeyOverride));

        if (referenceImage != null)
            _logger.LogWarning("gpt-image-1 does not support reference images, ignoring reference image parameter");

        var prompt = BuildImagePrompt(storyJson, tileText, extraInstructions);
        if (prompt.Length > MaxImagePromptLength)
            throw new ArgumentException($"Image prompt must be at most {MaxImagePromptLength} characters (current: {prompt.Length}). Shorten the story context or image instructions.", nameof(extraInstructions));

        var model = modelOverride ?? _model;
        var sizeConfig = !string.IsNullOrWhiteSpace(imageQualityOverride)
            ? ImageQualityToSize(imageQualityOverride.Trim())
            : _sizeConfig;
        var size = ParseImageSize(sizeConfig);
        var imageClient = new ImageClient(model, apiKeyOverride);

        _logger.LogInformation(
            "OpenAI Image (user key). Language: {LanguageCode}, Model: {Model}, Size: {Size}",
            languageCode, model, sizeConfig);

        var options = new ImageGenerationOptions { Size = size };
        var result = await imageClient.GenerateImageAsync(prompt, options, ct);
        var generatedImage = result.Value;

        if (generatedImage?.ImageBytes == null || generatedImage.ImageBytes.Length == 0)
            throw new InvalidOperationException("OpenAI returned empty image data");

        var imageBytes = generatedImage.ImageBytes.ToArray();
        var cropped = StoryImageAspectRatio.CropTo4x5(imageBytes);
        return (cropped, "image/png");
    }

    /// <summary>Maps light/medium/heavy to OpenAI size string (4:5 preserved via crop).</summary>
    private static string ImageQualityToSize(string imageQuality)
    {
        return imageQuality switch
        {
            "light" => "512x512",
            "heavy" => "1792x1024",
            _ => "1024x1024" // medium or unknown
        };
    }

    private static string BuildImagePrompt(string storyJson, string tileText, string? extraInstructions)
    {
        string? title = null, summary = null;
        try
        {
            using var doc = JsonDocument.Parse(storyJson);
            var root = doc.RootElement;
            title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;
        }
        catch { /* ignore */ }

        var sb = new StringBuilder();
        sb.Append("Create a colorful, child-friendly illustration for a children's story. ");
        if (!string.IsNullOrWhiteSpace(title))
            sb.Append($"Story: {title}. ");
        if (!string.IsNullOrWhiteSpace(summary))
            sb.Append($"Context: {(summary!.Length > 200 ? summary.Substring(0, 200) + "..." : summary)}. ");
        sb.Append($"Illustrate this scene: {tileText}. ");
        sb.Append("Style: warm, friendly, suitable for children. Bright colors, simple shapes, expressive characters.");
        if (!string.IsNullOrWhiteSpace(extraInstructions))
            sb.Append($" {extraInstructions}");
        return sb.ToString();
    }

    private static GeneratedImageSize ParseImageSize(string size)
    {
        return size switch
        {
            "256x256" => GeneratedImageSize.W256xH256,
            "512x512" => GeneratedImageSize.W512xH512,
            "1024x1024" => GeneratedImageSize.W1024xH1024,
            "1024x1792" => GeneratedImageSize.W1024xH1792,
            "1792x1024" => GeneratedImageSize.W1792xH1024,
            _ => GeneratedImageSize.W1024xH1024
        };
    }
}
