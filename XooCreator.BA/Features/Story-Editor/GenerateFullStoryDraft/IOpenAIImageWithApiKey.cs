namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// OpenAI story image generation with caller-provided API key.
/// Output is always 4:5 aspect ratio. Used by Generate Full Story Draft.
/// </summary>
public interface IOpenAIImageWithApiKey
{
    Task<(byte[] ImageData, string MimeType)> GenerateStoryImageAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions = null,
        byte[]? referenceImage = null,
        string? referenceImageMimeType = null,
        string? apiKeyOverride = null,
        string? modelOverride = null,
        string? imageQualityOverride = null,
        CancellationToken ct = default);
}
