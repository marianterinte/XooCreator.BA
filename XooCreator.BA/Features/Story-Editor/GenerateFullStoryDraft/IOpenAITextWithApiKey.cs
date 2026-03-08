namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// OpenAI full-story text generation with caller-provided API key.
/// Used by Generate Full Story Draft flow without modifying existing OpenAITextService.
/// </summary>
public interface IOpenAITextWithApiKey
{
    Task<string> GenerateFullStoryTextAsync(
        string title,
        string summary,
        string languageCode,
        int numberOfPages,
        List<string>? ageGroupIds = null,
        List<string>? topicIds = null,
        string? storyInstructions = null,
        string? apiKeyOverride = null,
        string? modelOverride = null,
        CancellationToken ct = default);
}
